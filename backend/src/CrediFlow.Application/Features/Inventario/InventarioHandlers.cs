using CrediFlow.Application.Common;
using CrediFlow.Application.DTOs;
using CrediFlow.Domain.Entities;
using CrediFlow.Domain.Enums;
using CrediFlow.Domain.Interfaces;
using MediatR;

namespace CrediFlow.Application.Features.Inventario;

// ─── QUERIES ────────────────────────────────────────────────────────
public record GetProductosQuery(string? Search = null, bool SoloActivos = false)
    : IRequest<Result<IEnumerable<ProductoDto>>>;

public record GetProductoByIdQuery(Guid Id) : IRequest<Result<ProductoDto>>;

// ─── COMMANDS ────────────────────────────────────────────────────────────────
public record CrearProductoCommand(
    string Nombre,
    string? Descripcion,
    decimal CostoUnitario,
    decimal PrecioVenta,
    int StockInicial,
    int StockMinimo,
    Guid? CategoriaId,
    Guid? ProveedorId,
    string? Foto,
    string? CodigoInterno,
    string? SKU,
    decimal? PrecioVentaMayorista = null,
    int? StockMaximo = null,
    Guid? UnidadMedidaId = null,
    bool PermiteVentaSinStock = false)
    : IRequest<Result<Guid>>;

public record ActualizarProductoCommand(
    Guid Id,
    string Nombre,
    string? Descripcion,
    decimal CostoUnitario,
    decimal PrecioVenta,
    int StockMinimo,
    Guid? CategoriaId,
    Guid? ProveedorId,
    string? Foto,
    string? CodigoInterno,
    string? SKU,
    decimal? PrecioVentaMayorista = null,
    int? StockMaximo = null,
    Guid? UnidadMedidaId = null,
    bool PermiteVentaSinStock = false)
    : IRequest<Result>;

public record EliminarProductoCommand(Guid Id) : IRequest<Result<Unit>>;
public record ReactivarProductoCommand(Guid Id) : IRequest<Result<Unit>>;

public record CargarFacturaItem(Guid ProductoId, int Cantidad, decimal CostoUnitario);

public record CargarFacturaCompraCommand(
    Guid ProveedorId,
    string NumeroFactura,
    List<CargarFacturaItem> Items,
    bool ActualizarCostos = false)
    : IRequest<Result>;

// ─── HANDLERS ────────────────────────────────────────────────────────────────
public class InventarioHandlers :
    IRequestHandler<GetProductosQuery, Result<IEnumerable<ProductoDto>>>,
    IRequestHandler<GetProductoByIdQuery, Result<ProductoDto>>,
    IRequestHandler<CrearProductoCommand, Result<Guid>>,
    IRequestHandler<ActualizarProductoCommand, Result>,
    IRequestHandler<EliminarProductoCommand, Result<Unit>>,
    IRequestHandler<ReactivarProductoCommand, Result<Unit>>,
    IRequestHandler<CargarFacturaCompraCommand, Result>
{
    private readonly IUnitOfWork _uow;
    public InventarioHandlers(IUnitOfWork uow) => _uow = uow;

    public async Task<Result<IEnumerable<ProductoDto>>> Handle(GetProductosQuery request, CancellationToken ct)
    {
        var productos = await _uow.Productos.GetAllAsync(ct);

        if (request.SoloActivos)
            productos = productos.Where(p => p.Activo);

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var s = request.Search.ToLower();
            productos = productos.Where(p =>
                p.Nombre.ToLower().Contains(s) ||
                (p.SKU != null && p.SKU.ToLower().Contains(s)) ||
                (p.CodigoInterno != null && p.CodigoInterno.ToLower().Contains(s)));
        }

        var dtos = productos.Select(p => ToDto(p)).OrderBy(p => p.Nombre);
        return Result<IEnumerable<ProductoDto>>.Success(dtos);
    }

    public async Task<Result<ProductoDto>> Handle(GetProductoByIdQuery request, CancellationToken ct)
    {
        var p = await _uow.Productos.GetByIdAsync(request.Id, ct);
        if (p == null) return Result<ProductoDto>.Failure("Producto no encontrado.");
        return Result<ProductoDto>.Success(ToDto(p));
    }

    public async Task<Result<Guid>> Handle(CrearProductoCommand req, CancellationToken ct)
    {
        var prod = Producto.Crear(
            req.Nombre, req.CostoUnitario, req.PrecioVenta,
            req.StockInicial, req.StockMinimo, req.Descripcion,
            req.Foto, req.ProveedorId, req.CategoriaId,
            req.CodigoInterno, req.SKU,
            req.PrecioVentaMayorista, req.StockMaximo,
            req.UnidadMedidaId, req.PermiteVentaSinStock);

        await _uow.Productos.AddAsync(prod, ct);
        await _uow.SaveChangesAsync(ct);
        return Result<Guid>.Success(prod.Id);
    }

    public async Task<Result> Handle(ActualizarProductoCommand req, CancellationToken ct)
    {
        var prod = await _uow.Productos.GetByIdAsync(req.Id, ct);
        if (prod == null) return Result.Failure("Producto no encontrado.");

        prod.Actualizar(
            req.Nombre, req.Descripcion, req.CostoUnitario, req.PrecioVenta,
            req.StockMinimo, req.CategoriaId, req.ProveedorId,
            req.Foto, req.CodigoInterno, req.SKU,
            req.PrecioVentaMayorista, req.StockMaximo,
            req.UnidadMedidaId, req.PermiteVentaSinStock);

        _uow.Productos.Update(prod);
        await _uow.SaveChangesAsync(ct);
        return Result.Success();
    }

    public async Task<Result<Unit>> Handle(ReactivarProductoCommand request, CancellationToken ct)
    {
        var prod = await _uow.Productos.GetByIdAsync(request.Id, ct);
        if (prod == null) return Result<Unit>.Failure("Producto no encontrado.");
        prod.Activar();
        await _uow.SaveChangesAsync(ct);
        return Result<Unit>.Success(Unit.Value);
    }

    public async Task<Result<Unit>> Handle(EliminarProductoCommand request, CancellationToken ct)
    {
        var prod = await _uow.Productos.GetByIdAsync(request.Id, ct);
        if (prod == null) return Result<Unit>.Failure("Producto no encontrado.");
        prod.Desactivar();
        await _uow.SaveChangesAsync(ct);
        return Result<Unit>.Success(Unit.Value);
    }

    public async Task<Result> Handle(CargarFacturaCompraCommand request, CancellationToken ct)
    {
        if (request.Items == null || !request.Items.Any())
            return Result.Failure("La factura no contiene ítems.");

        foreach (var item in request.Items)
        {
            var p = await _uow.Productos.GetByIdAsync(item.ProductoId, ct);
            if (p == null) continue;

            if (request.ActualizarCostos)
                p.ActualizarPrecios(item.CostoUnitario, p.PrecioVenta, p.PrecioVentaMayorista);

            p.AjustarStock(item.Cantidad, TipoMovimientoStock.COMPRA, $"Factura {request.NumeroFactura}");

            var mov = new MovimientoStock
            {
                ProductoId = p.Id,
                Tipo = TipoMovimientoStock.COMPRA,
                Cantidad = item.Cantidad,
                CostoUnitario = item.CostoUnitario,
                Motivo = $"Factura compra: {request.NumeroFactura}",
                ProveedorId = request.ProveedorId,
                NumeroFactura = request.NumeroFactura
            };

            await _uow.Productos.AddMovimientoAsync(mov, ct);
            _uow.Productos.Update(p);
        }

        await _uow.SaveChangesAsync(ct);
        return Result.Success();
    }

    private static ProductoDto ToDto(Producto p) => new ProductoDto(
        p.Id, p.Nombre, p.CodigoInterno, p.SKU, p.Descripcion, p.Foto,
        p.CostoUnitario, p.PrecioVenta, p.PrecioVentaMayorista,
        p.StockActual, p.StockMinimo, p.StockMaximo,
        p.UnidadMedidaId, p.UnidadMedida?.Nombre, p.BajoStock, p.MargenGanancia,
        p.Activo, p.PermiteVentaSinStock,
        p.ProveedorId, p.Proveedor?.RazonSocial,
        p.CategoriaId, p.Categoria?.Nombre);
}
