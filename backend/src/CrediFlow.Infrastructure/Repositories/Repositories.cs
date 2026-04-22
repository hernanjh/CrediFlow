using CrediFlow.Domain.Entities;
using CrediFlow.Domain.Enums;
using CrediFlow.Domain.Interfaces;
using CrediFlow.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CrediFlow.Infrastructure.Repositories;

public class ClienteRepository : IClienteRepository
{
    private readonly CrediFlowDbContext _ctx;
    public ClienteRepository(CrediFlowDbContext ctx) => _ctx = ctx;

    public async Task<Cliente?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await _ctx.Clientes.Include(c => c.Zona).Include(c => c.Promesas)
            .FirstOrDefaultAsync(c => c.Id == id, ct);

    public async Task<Cliente?> GetByDniAsync(string dni, CancellationToken ct = default)
        => await _ctx.Clientes.FirstOrDefaultAsync(c => c.DNI == dni, ct);

    public async Task<IEnumerable<Cliente>> GetAllAsync(CancellationToken ct = default)
        => await _ctx.Clientes.Include(c => c.Zona).AsNoTracking().ToListAsync(ct);

    public async Task<IEnumerable<Cliente>> GetByZonaAsync(Guid zonaId, CancellationToken ct = default)
        => await _ctx.Clientes.Where(c => c.ZonaId == zonaId).Include(c => c.Zona).ToListAsync(ct);

    public async Task<IEnumerable<Cliente>> GetByEstadoAsync(EstadoCliente estado, CancellationToken ct = default)
        => await _ctx.Clientes.Where(c => c.Estado == estado).ToListAsync(ct);

    public async Task AddAsync(Cliente cliente, CancellationToken ct = default)
        => await _ctx.Clientes.AddAsync(cliente, ct);

    public void Update(Cliente cliente) => _ctx.Clientes.Update(cliente);

    public async Task<int> CountAsync(CancellationToken ct = default)
        => await _ctx.Clientes.CountAsync(ct);
}

public class CreditoRepository : ICreditoRepository
{
    private readonly CrediFlowDbContext _ctx;
    public CreditoRepository(CrediFlowDbContext ctx) => _ctx = ctx;

    public async Task<Credito?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await _ctx.Creditos.Include(c => c.Cliente).Include(c => c.Vendedor)
            .FirstOrDefaultAsync(c => c.Id == id, ct);

    public async Task<Credito?> GetByIdWithCuotasAsync(Guid id, CancellationToken ct = default)
        => await _ctx.Creditos.Include(c => c.Cuotas).Include(c => c.Cliente)
            .FirstOrDefaultAsync(c => c.Id == id, ct);

    public async Task<IEnumerable<Credito>> GetByClienteAsync(Guid clienteId, CancellationToken ct = default)
        => await _ctx.Creditos
            .Include(c => c.Cuotas)
            .Include(c => c.Cliente)
            .Include(c => c.Vendedor)
            .Where(c => c.ClienteId == clienteId)
            .ToListAsync(ct);

    public async Task<IEnumerable<Credito>> GetActivosAsync(CancellationToken ct = default)
        => await _ctx.Creditos
            .Include(c => c.Cliente)
            .Include(c => c.Vendedor)
            .Include(c => c.Cuotas)
            .Where(c => c.Estado != EstadoCredito.CANCELADO)
            .ToListAsync(ct);

    public async Task<IEnumerable<Credito>> GetConMoraAsync(CancellationToken ct = default)
        => await _ctx.Creditos
            .Include(c => c.Cuotas)
            .Include(c => c.Cliente)
                .ThenInclude(cl => cl!.Zona)
            .Where(c => c.Estado == EstadoCredito.EN_MORA || c.Estado == EstadoCredito.EN_MORA_GRAVE)
            .ToListAsync(ct);

    public async Task AddAsync(Credito credito, CancellationToken ct = default)
        => await _ctx.Creditos.AddAsync(credito, ct);

    public void Update(Credito credito) => _ctx.Creditos.Update(credito);

    public async Task<decimal> GetCapitalActivoTotalAsync(CancellationToken ct = default)
        => await _ctx.Creditos.Where(c => c.Estado != EstadoCredito.CANCELADO)
            .SumAsync(c => c.SaldoPendiente, ct);
}

public class CuotaRepository : ICuotaRepository
{
    private readonly CrediFlowDbContext _ctx;
    public CuotaRepository(CrediFlowDbContext ctx) => _ctx = ctx;

    public async Task<Cuota?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await _ctx.Cuotas.Include(c => c.Credito).ThenInclude(cr => cr!.Cliente)
            .FirstOrDefaultAsync(c => c.Id == id, ct);

    public async Task<IEnumerable<Cuota>> GetByCreditoAsync(Guid creditoId, CancellationToken ct = default)
        => await _ctx.Cuotas.Where(c => c.CreditoId == creditoId).OrderBy(c => c.NumeroCuota).ToListAsync(ct);

    public async Task<IEnumerable<Cuota>> GetVencidasAsync(CancellationToken ct = default)
        => await _ctx.Cuotas
            .Where(c => c.FechaVencimiento.Date < DateTime.UtcNow.Date &&
                        c.Estado != EstadoCuota.PAGADA)
            .Include(c => c.Credito)
            .ToListAsync(ct);

    public async Task<IEnumerable<Cuota>> GetPorVencerAsync(int dias, CancellationToken ct = default)
    {
        var limite = DateTime.UtcNow.Date.AddDays(dias);
        return await _ctx.Cuotas
            .Where(c => c.FechaVencimiento.Date <= limite &&
                        c.FechaVencimiento.Date >= DateTime.UtcNow.Date &&
                        c.Estado != EstadoCuota.PAGADA)
            .ToListAsync(ct);
    }

    public async Task<IEnumerable<Cuota>> GetHojaRutaVendedorAsync(
        Guid vendedorId, DateTime fecha, CancellationToken ct = default)
    {
        // La hoja de ruta incluye:
        // 1. Cuotas vencidas no pagadas del vendedor
        // 2. Cuotas con vencimiento hoy
        return await _ctx.Cuotas
            .Include(c => c.Credito)
                .ThenInclude(cr => cr!.Cliente)
                    .ThenInclude(cl => cl!.Zona)
            .Include(c => c.Credito)
                .ThenInclude(cr => cr!.Cliente)
                    .ThenInclude(cl => cl!.Promesas)
            .Where(c =>
                c.Credito!.VendedorId == vendedorId &&
                c.Estado != EstadoCuota.PAGADA &&
                c.FechaVencimiento.Date <= fecha.Date)
            .OrderBy(c => c.Credito!.Cliente!.Direccion)
            .ToListAsync(ct);
    }

    public async Task AddRangeAsync(IEnumerable<Cuota> cuotas, CancellationToken ct = default)
        => await _ctx.Cuotas.AddRangeAsync(cuotas, ct);

    public void Update(Cuota cuota) => _ctx.Cuotas.Update(cuota);
}

public class PagoCobradoRepository : IPagoCobradoRepository
{
    private readonly CrediFlowDbContext _ctx;
    public PagoCobradoRepository(CrediFlowDbContext ctx) => _ctx = ctx;

    public async Task<PagoCobrado?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await _ctx.PagosCobrados.FindAsync(new object[] { id }, ct);

    public async Task<IEnumerable<PagoCobrado>> GetByVendedorFechaAsync(
        Guid vendedorId, DateTime fecha, CancellationToken ct = default)
        => await _ctx.PagosCobrados
            .Where(p => p.VendedorId == vendedorId && p.FechaPago.Date == fecha.Date)
            .ToListAsync(ct);

    public async Task<decimal> GetTotalCobradoVendedorHoyAsync(Guid vendedorId, CancellationToken ct = default)
        => await _ctx.PagosCobrados
            .Where(p => p.VendedorId == vendedorId && p.FechaPago.Date == DateTime.UtcNow.Date)
            .SumAsync(p => p.Monto, ct);

    public async Task AddAsync(PagoCobrado pago, CancellationToken ct = default)
        => await _ctx.PagosCobrados.AddAsync(pago, ct);

    public async Task AddRangeAsync(IEnumerable<PagoCobrado> pagos, CancellationToken ct = default)
        => await _ctx.PagosCobrados.AddRangeAsync(pagos, ct);
}

public class CierreCajaRepository : ICierreCajaRepository
{
    private readonly CrediFlowDbContext _ctx;
    public CierreCajaRepository(CrediFlowDbContext ctx) => _ctx = ctx;

    public async Task<CierreCaja?> GetByVendedorFechaAsync(Guid vendedorId, DateTime fecha, CancellationToken ct = default)
        => await _ctx.CierresCaja
            .FirstOrDefaultAsync(c => c.VendedorId == vendedorId && c.Fecha == fecha.Date, ct);

    public async Task<IEnumerable<CierreCaja>> GetByFechaAsync(DateTime fecha, CancellationToken ct = default)
        => await _ctx.CierresCaja.Include(c => c.Vendedor)
            .Where(c => c.Fecha == fecha.Date).ToListAsync(ct);

    public async Task<IEnumerable<CierreCaja>> GetConDiferenciaAsync(CancellationToken ct = default)
        => await _ctx.CierresCaja
            .Where(c => c.Estado == EstadoCierreCaja.FALTANTE || c.Estado == EstadoCierreCaja.CON_DIFERENCIA)
            .ToListAsync(ct);

    public async Task AddAsync(CierreCaja cierre, CancellationToken ct = default)
        => await _ctx.CierresCaja.AddAsync(cierre, ct);

    public void Update(CierreCaja cierre) => _ctx.CierresCaja.Update(cierre);
}

public class ProductoRepository : IProductoRepository
{
    private readonly CrediFlowDbContext _ctx;
    public ProductoRepository(CrediFlowDbContext ctx) => _ctx = ctx;

    public async Task<Producto?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await _ctx.Productos
            .Include(p => p.Categoria)
            .Include(p => p.Proveedor)
            .FirstOrDefaultAsync(p => p.Id == id, ct);

    public async Task<IEnumerable<Producto>> GetAllAsync(CancellationToken ct = default)
        => await _ctx.Productos
            .Include(p => p.Categoria)
            .Include(p => p.Proveedor)
            .Where(p => p.Activo)
            .ToListAsync(ct);

    public async Task<IEnumerable<Producto>> GetBajoStockAsync(CancellationToken ct = default)
        => await _ctx.Productos
            .Include(p => p.Categoria)
            .Include(p => p.Proveedor)
            .Where(p => p.Activo && p.StockActual <= p.StockMinimo)
            .ToListAsync(ct);

    public async Task AddAsync(Producto producto, CancellationToken ct = default)
        => await _ctx.Productos.AddAsync(producto, ct);

    public void Update(Producto producto) => _ctx.Productos.Update(producto);

    public async Task AddMovimientoAsync(MovimientoStock movimiento, CancellationToken ct = default)
        => await _ctx.MovimientosStock.AddAsync(movimiento, ct);
}

public class ProveedorRepository : IProveedorRepository
{
    private readonly CrediFlowDbContext _ctx;
    public ProveedorRepository(CrediFlowDbContext ctx) => _ctx = ctx;

    public async Task<Proveedor?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await _ctx.Proveedores.FindAsync(new object[] { id }, ct);

    public async Task<IEnumerable<Proveedor>> GetAllAsync(CancellationToken ct = default)
        => await _ctx.Proveedores.Where(p => p.Activo).ToListAsync(ct);

    public async Task AddAsync(Proveedor proveedor, CancellationToken ct = default)
        => await _ctx.Proveedores.AddAsync(proveedor, ct);

    public void Update(Proveedor proveedor) => _ctx.Proveedores.Update(proveedor);
}

public class CategoriaProductoRepository : ICategoriaProductoRepository
{
    private readonly CrediFlowDbContext _ctx;
    public CategoriaProductoRepository(CrediFlowDbContext ctx) => _ctx = ctx;

    public async Task<CategoriaProducto?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await _ctx.CategoriasProducto.FindAsync(new object[] { id }, ct);

    public async Task<IEnumerable<CategoriaProducto>> GetAllAsync(CancellationToken ct = default)
        => await _ctx.CategoriasProducto.Where(c => c.Activa).ToListAsync(ct);

    public async Task AddAsync(CategoriaProducto categoria, CancellationToken ct = default)
        => await _ctx.CategoriasProducto.AddAsync(categoria, ct);

    public void Update(CategoriaProducto categoria) => _ctx.CategoriasProducto.Update(categoria);
}

public class UsuarioRepository : IUsuarioRepository
{
    private readonly CrediFlowDbContext _ctx;
    public UsuarioRepository(CrediFlowDbContext ctx) => _ctx = ctx;

    public async Task<Usuario?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await _ctx.Usuarios.Include(u => u.Zonas).FirstOrDefaultAsync(u => u.Id == id, ct);

    public async Task<Usuario?> GetByEmailAsync(string email, CancellationToken ct = default)
        => await _ctx.Usuarios.FirstOrDefaultAsync(u => u.Email == email.ToLower(), ct);

    public async Task<IEnumerable<Usuario>> GetAllAsync(CancellationToken ct = default)
        => await _ctx.Usuarios.AsNoTracking().ToListAsync(ct);

    public async Task<IEnumerable<Usuario>> GetVendedoresAsync(CancellationToken ct = default)
        => await _ctx.Usuarios.Where(u => u.Rol == TipoUsuario.VENDEDOR && u.Activo).ToListAsync(ct);

    public async Task AddAsync(Usuario usuario, CancellationToken ct = default)
        => await _ctx.Usuarios.AddAsync(usuario, ct);

    public void Update(Usuario usuario) => _ctx.Usuarios.Update(usuario);

    public async Task<RefreshToken?> GetRefreshTokenAsync(string token, CancellationToken ct = default)
        => await _ctx.RefreshTokens.Include(rt => rt.Usuario)
            .FirstOrDefaultAsync(rt => rt.Token == token, ct);

    public async Task AddRefreshTokenAsync(RefreshToken token, CancellationToken ct = default)
        => await _ctx.RefreshTokens.AddAsync(token, ct);
}

public class ZonaRepository : IZonaRepository
{
    private readonly CrediFlowDbContext _ctx;
    public ZonaRepository(CrediFlowDbContext ctx) => _ctx = ctx;

    public async Task<Zona?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await _ctx.Zonas.FindAsync(new object[] { id }, ct);

    public async Task<IEnumerable<Zona>> GetAllAsync(CancellationToken ct = default)
        => await _ctx.Zonas.Where(z => z.Activa).ToListAsync(ct);

    public async Task AddAsync(Zona zona, CancellationToken ct = default)
        => await _ctx.Zonas.AddAsync(zona, ct);

    public void Update(Zona zona) => _ctx.Zonas.Update(zona);
}

public class FeriadoRepository : IFeriadoRepository
{
    private readonly CrediFlowDbContext _ctx;
    public FeriadoRepository(CrediFlowDbContext ctx) => _ctx = ctx;

    public async Task<IEnumerable<Feriado>> GetByRangoAsync(DateTime desde, DateTime hasta, CancellationToken ct = default)
        => await _ctx.Feriados.Where(f => f.Activo && f.Fecha >= desde && f.Fecha <= hasta).ToListAsync(ct);

    public async Task<bool> EsFeriadoAsync(DateTime fecha, CancellationToken ct = default)
        => await _ctx.Feriados.AnyAsync(f => f.Activo && f.Fecha.Date == fecha.Date, ct);

    public async Task AddAsync(Feriado feriado, CancellationToken ct = default)
        => await _ctx.Feriados.AddAsync(feriado, ct);
}

public class AlertaRepository : IAlertaRepository
{
    private readonly CrediFlowDbContext _ctx;
    public AlertaRepository(CrediFlowDbContext ctx) => _ctx = ctx;

    public async Task<IEnumerable<AlertaOperativa>> GetNoLeidasAsync(
        Guid? usuarioId = null, CancellationToken ct = default)
        => await _ctx.Alertas
            .Where(a => !a.Leida && (a.DestinoUsuarioId == null || a.DestinoUsuarioId == usuarioId))
            .OrderByDescending(a => a.CreatedAt)
            .Take(20)
            .ToListAsync(ct);

    public async Task AddAsync(AlertaOperativa alerta, CancellationToken ct = default)
        => await _ctx.Alertas.AddAsync(alerta, ct);

    public void Update(AlertaOperativa alerta) => _ctx.Alertas.Update(alerta);
}

public class ConfiguracionRepository : IConfiguracionRepository
{
    private readonly CrediFlowDbContext _ctx;
    public ConfiguracionRepository(CrediFlowDbContext ctx) => _ctx = ctx;

    public async Task<ConfiguracionGlobal?> GetAsync(CancellationToken ct = default)
        => await _ctx.Configuraciones.FirstOrDefaultAsync(ct);

    public void Update(ConfiguracionGlobal config) => _ctx.Configuraciones.Update(config);
}

public class PerfilRepository : IPerfilRepository
{
    private readonly CrediFlowDbContext _ctx;
    public PerfilRepository(CrediFlowDbContext ctx) => _ctx = ctx;

    public async Task<Perfil?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await _ctx.Perfiles.Include(p => p.Permisos).ThenInclude(pp => pp.Permiso)
            .FirstOrDefaultAsync(p => p.Id == id, ct);

    public async Task<IEnumerable<Perfil>> GetAllAsync(CancellationToken ct = default)
        => await _ctx.Perfiles.Where(p => p.Activo).ToListAsync(ct);

    public async Task<IEnumerable<Permiso>> GetAllPermisosAsync(CancellationToken ct = default)
        => await _ctx.Permisos.Where(p => p.Activo).ToListAsync(ct);

    public async Task AddAsync(Perfil perfil, CancellationToken ct = default)
        => await _ctx.Perfiles.AddAsync(perfil, ct);

    public void Update(Perfil perfil) => _ctx.Perfiles.Update(perfil);
}

public class VentaRepository : IVentaRepository
{
    private readonly CrediFlowDbContext _ctx;
    public VentaRepository(CrediFlowDbContext ctx) => _ctx = ctx;

    public async Task<Venta?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await _ctx.Ventas.Include(v => v.Items).ThenInclude(i => i.Producto)
            .FirstOrDefaultAsync(v => v.Id == id, ct);

    public async Task<IEnumerable<Venta>> GetAllAsync(CancellationToken ct = default)
        => await _ctx.Ventas.Include(v => v.Items).ToListAsync(ct);

    public async Task<IEnumerable<Venta>> GetHoyAsync(CancellationToken ct = default)
        => await _ctx.Ventas.Where(v => v.Fecha.Date == DateTime.UtcNow.Date).ToListAsync(ct);

    public async Task AddAsync(Venta venta, CancellationToken ct = default)
        => await _ctx.Ventas.AddAsync(venta, ct);
}

public class ListaPrecioRepository : IListaPrecioRepository
{
    private readonly CrediFlowDbContext _ctx;
    public ListaPrecioRepository(CrediFlowDbContext ctx) => _ctx = ctx;

    public async Task<ListaPrecio?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await _ctx.ListasPrecios
            .Include(l => l.Items)
                .ThenInclude(i => i.Producto)
            .FirstOrDefaultAsync(l => l.Id == id, ct);

    public async Task<IEnumerable<ListaPrecio>> GetAllAsync(CancellationToken ct = default)
        => await _ctx.ListasPrecios
            .Where(l => l.Activa)
            .Include(l => l.Items)
                .ThenInclude(i => i.Producto)
            .ToListAsync(ct);

    public async Task AddAsync(ListaPrecio lista, CancellationToken ct = default)
        => await _ctx.ListasPrecios.AddAsync(lista, ct);

    public async Task AddItemAsync(ListaPrecioItem item, CancellationToken ct = default)
        => await _ctx.ListasPreciosItems.AddAsync(item, ct);

    public async Task<ListaPrecioItem?> GetItemByIdAsync(Guid id, CancellationToken ct = default)
        => await _ctx.ListasPreciosItems.FirstOrDefaultAsync(i => i.Id == id, ct);

    public void RemoveItem(ListaPrecioItem item) => _ctx.ListasPreciosItems.Remove(item);

    public void Update(ListaPrecio lista) => _ctx.ListasPrecios.Update(lista);
}
