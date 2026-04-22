using CrediFlow.Domain.Entities;
using CrediFlow.Domain.Enums;

namespace CrediFlow.Domain.Interfaces;

public interface IClienteRepository
{
    Task<Cliente?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<Cliente?> GetByDniAsync(string dni, CancellationToken ct = default);
    Task<IEnumerable<Cliente>> GetAllAsync(CancellationToken ct = default);
    Task<IEnumerable<Cliente>> GetByZonaAsync(Guid zonaId, CancellationToken ct = default);
    Task<IEnumerable<Cliente>> GetByEstadoAsync(EstadoCliente estado, CancellationToken ct = default);
    Task AddAsync(Cliente cliente, CancellationToken ct = default);
    void Update(Cliente cliente);
    Task<int> CountAsync(CancellationToken ct = default);
}

public interface ICreditoRepository
{
    Task<Credito?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<Credito?> GetByIdWithCuotasAsync(Guid id, CancellationToken ct = default);
    Task<IEnumerable<Credito>> GetByClienteAsync(Guid clienteId, CancellationToken ct = default);
    Task<IEnumerable<Credito>> GetActivosAsync(CancellationToken ct = default);
    Task<IEnumerable<Credito>> GetConMoraAsync(CancellationToken ct = default);
    Task AddAsync(Credito credito, CancellationToken ct = default);
    void Update(Credito credito);
    Task<decimal> GetCapitalActivoTotalAsync(CancellationToken ct = default);
}

public interface ICuotaRepository
{
    Task<Cuota?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IEnumerable<Cuota>> GetByCreditoAsync(Guid creditoId, CancellationToken ct = default);
    Task<IEnumerable<Cuota>> GetVencidasAsync(CancellationToken ct = default);
    Task<IEnumerable<Cuota>> GetPorVencerAsync(int dias, CancellationToken ct = default);
    Task<IEnumerable<Cuota>> GetHojaRutaVendedorAsync(Guid vendedorId, DateTime fecha, CancellationToken ct = default);
    Task AddRangeAsync(IEnumerable<Cuota> cuotas, CancellationToken ct = default);
    void Update(Cuota cuota);
}

public interface IPagoCobradoRepository
{
    Task<PagoCobrado?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IEnumerable<PagoCobrado>> GetByVendedorFechaAsync(Guid vendedorId, DateTime fecha, CancellationToken ct = default);
    Task<decimal> GetTotalCobradoVendedorHoyAsync(Guid vendedorId, CancellationToken ct = default);
    Task AddAsync(PagoCobrado pago, CancellationToken ct = default);
    Task AddRangeAsync(IEnumerable<PagoCobrado> pagos, CancellationToken ct = default);
}

public interface ICierreCajaRepository
{
    Task<CierreCaja?> GetByVendedorFechaAsync(Guid vendedorId, DateTime fecha, CancellationToken ct = default);
    Task<IEnumerable<CierreCaja>> GetByFechaAsync(DateTime fecha, CancellationToken ct = default);
    Task<IEnumerable<CierreCaja>> GetConDiferenciaAsync(CancellationToken ct = default);
    Task AddAsync(CierreCaja cierre, CancellationToken ct = default);
    void Update(CierreCaja cierre);
}

public interface IProductoRepository
{
    Task<Producto?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IEnumerable<Producto>> GetAllAsync(CancellationToken ct = default);
    Task<IEnumerable<Producto>> GetBajoStockAsync(CancellationToken ct = default);
    Task AddAsync(Producto producto, CancellationToken ct = default);
    void Update(Producto producto);
    Task AddMovimientoAsync(MovimientoStock movimiento, CancellationToken ct = default);
}

public interface IProveedorRepository
{
    Task<Proveedor?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IEnumerable<Proveedor>> GetAllAsync(CancellationToken ct = default);
    Task AddAsync(Proveedor proveedor, CancellationToken ct = default);
    void Update(Proveedor proveedor);
}

public interface ICategoriaProductoRepository
{
    Task<CategoriaProducto?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IEnumerable<CategoriaProducto>> GetAllAsync(CancellationToken ct = default);
    Task AddAsync(CategoriaProducto categoria, CancellationToken ct = default);
    void Update(CategoriaProducto categoria);
}

public interface IConfiguracionRepository
{
    Task<ConfiguracionGlobal?> GetAsync(CancellationToken ct = default);
    void Update(ConfiguracionGlobal config);
}

public interface IPerfilRepository
{
    Task<Perfil?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IEnumerable<Perfil>> GetAllAsync(CancellationToken ct = default);
    Task<IEnumerable<Permiso>> GetAllPermisosAsync(CancellationToken ct = default);
    Task AddAsync(Perfil perfil, CancellationToken ct = default);
    void Update(Perfil perfil);
}

public interface IVentaRepository
{
    Task<Venta?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IEnumerable<Venta>> GetAllAsync(CancellationToken ct = default);
    Task<IEnumerable<Venta>> GetHoyAsync(CancellationToken ct = default);
    Task AddAsync(Venta venta, CancellationToken ct = default);
}

public interface IListaPrecioRepository
{
    Task<ListaPrecio?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IEnumerable<ListaPrecio>> GetAllAsync(CancellationToken ct = default);
    Task AddAsync(ListaPrecio lista, CancellationToken ct = default);
    Task AddItemAsync(ListaPrecioItem item, CancellationToken ct = default);
    Task<ListaPrecioItem?> GetItemByIdAsync(Guid id, CancellationToken ct = default);
    void RemoveItem(ListaPrecioItem item);
    void Update(ListaPrecio lista);
}

public interface IUsuarioRepository
{
    Task<Usuario?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<Usuario?> GetByEmailAsync(string email, CancellationToken ct = default);
    Task<IEnumerable<Usuario>> GetAllAsync(CancellationToken ct = default);
    Task<IEnumerable<Usuario>> GetVendedoresAsync(CancellationToken ct = default);
    Task AddAsync(Usuario usuario, CancellationToken ct = default);
    void Update(Usuario usuario);
    Task<RefreshToken?> GetRefreshTokenAsync(string token, CancellationToken ct = default);
    Task AddRefreshTokenAsync(RefreshToken token, CancellationToken ct = default);
}

public interface IZonaRepository
{
    Task<Zona?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IEnumerable<Zona>> GetAllAsync(CancellationToken ct = default);
    Task AddAsync(Zona zona, CancellationToken ct = default);
    void Update(Zona zona);
}

public interface IFeriadoRepository
{
    Task<IEnumerable<Feriado>> GetByRangoAsync(DateTime desde, DateTime hasta, CancellationToken ct = default);
    Task<bool> EsFeriadoAsync(DateTime fecha, CancellationToken ct = default);
    Task AddAsync(Feriado feriado, CancellationToken ct = default);
}

public interface IAlertaRepository
{
    Task<IEnumerable<AlertaOperativa>> GetNoLeidasAsync(Guid? usuarioId = null, CancellationToken ct = default);
    Task AddAsync(AlertaOperativa alerta, CancellationToken ct = default);
    void Update(AlertaOperativa alerta);
}

public interface IUnitOfWork
{
    IClienteRepository Clientes { get; }
    ICreditoRepository Creditos { get; }
    ICuotaRepository Cuotas { get; }
    IPagoCobradoRepository Pagos { get; }
    ICierreCajaRepository CierresCaja { get; }
    IProductoRepository Productos { get; }
    IProveedorRepository Proveedores { get; }
    ICategoriaProductoRepository CategoriasProducto { get; }
    IConfiguracionRepository Configuracion { get; }
    IPerfilRepository Perfiles { get; }
    IVentaRepository Ventas { get; }
    IListaPrecioRepository ListasPrecios { get; }
    IUsuarioRepository Usuarios { get; }
    IZonaRepository Zonas { get; }
    IFeriadoRepository Feriados { get; }
    IAlertaRepository Alertas { get; }

    Task<int> SaveChangesAsync(CancellationToken ct = default);
    Task BeginTransactionAsync(CancellationToken ct = default);
    Task CommitTransactionAsync(CancellationToken ct = default);
    Task RollbackTransactionAsync(CancellationToken ct = default);
}
