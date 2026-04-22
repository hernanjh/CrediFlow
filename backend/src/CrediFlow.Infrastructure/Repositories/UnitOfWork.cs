using CrediFlow.Domain.Interfaces;
using CrediFlow.Infrastructure.Persistence;
using CrediFlow.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore.Storage;

namespace CrediFlow.Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly CrediFlowDbContext _ctx;
    private IDbContextTransaction? _transaction;

    public IClienteRepository Clientes { get; }
    public ICreditoRepository Creditos { get; }
    public ICuotaRepository Cuotas { get; }
    public IPagoCobradoRepository Pagos { get; }
    public ICierreCajaRepository CierresCaja { get; }
    public IProductoRepository Productos { get; }
    public IProveedorRepository Proveedores { get; }
    public ICategoriaProductoRepository CategoriasProducto { get; }
    public IConfiguracionRepository Configuracion { get; }
    public IPerfilRepository Perfiles { get; }
    public IVentaRepository Ventas { get; }
    public IListaPrecioRepository ListasPrecios { get; }
    public IUsuarioRepository Usuarios { get; }
    public IZonaRepository Zonas { get; }
    public IFeriadoRepository Feriados { get; }
    public IAlertaRepository Alertas { get; }

    public UnitOfWork(
        CrediFlowDbContext ctx,
        IClienteRepository clientes,
        ICreditoRepository creditos,
        ICuotaRepository cuotas,
        IPagoCobradoRepository pagos,
        ICierreCajaRepository cierresCaja,
        IProductoRepository productos,
        IProveedorRepository proveedores,
        ICategoriaProductoRepository categoriasProducto,
        IConfiguracionRepository configuracion,
        IPerfilRepository perfiles,
        IVentaRepository ventas,
        IListaPrecioRepository listasPrecios,
        IUsuarioRepository usuarios,
        IZonaRepository zonas,
        IFeriadoRepository feriados,
        IAlertaRepository alertas)
    {
        _ctx = ctx;
        Clientes = clientes;
        Creditos = creditos;
        Cuotas = cuotas;
        Pagos = pagos;
        CierresCaja = cierresCaja;
        Productos = productos;
        Proveedores = proveedores;
        CategoriasProducto = categoriasProducto;
        Configuracion = configuracion;
        Perfiles = perfiles;
        Ventas = ventas;
        ListasPrecios = listasPrecios;
        Usuarios = usuarios;
        Zonas = zonas;
        Feriados = feriados;
        Alertas = alertas;
    }

    public async Task<int> SaveChangesAsync(CancellationToken ct = default)
        => await _ctx.SaveChangesAsync(ct);

    public async Task BeginTransactionAsync(CancellationToken ct = default)
        => _transaction = await _ctx.Database.BeginTransactionAsync(ct);

    public async Task CommitTransactionAsync(CancellationToken ct = default)
    {
        if (_transaction != null)
        {
            await _transaction.CommitAsync(ct);
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public async Task RollbackTransactionAsync(CancellationToken ct = default)
    {
        if (_transaction != null)
        {
            await _transaction.RollbackAsync(ct);
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }
}
