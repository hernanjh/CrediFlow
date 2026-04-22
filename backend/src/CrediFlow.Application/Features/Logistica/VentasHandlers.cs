using CrediFlow.Application.Common;
using CrediFlow.Domain.Entities;
using CrediFlow.Domain.Enums;
using CrediFlow.Domain.Interfaces;
using MediatR;

namespace CrediFlow.Application.Features.Logistica;

public record VentaItemInput(Guid ProductoId, int Cantidad, decimal PrecioUnitario);

public record RegistrarVentaContadoCommand(
    Guid VendedorId,
    Guid? ClienteId,
    Guid? FormaCobroId,
    List<VentaItemInput> Items) : IRequest<Result<Guid>>;

public class RegistrarVentaContadoHandler : IRequestHandler<RegistrarVentaContadoCommand, Result<Guid>>
{
    private readonly IUnitOfWork _uow;
    public RegistrarVentaContadoHandler(IUnitOfWork uow) => _uow = uow;

    public async Task<Result<Guid>> Handle(RegistrarVentaContadoCommand req, CancellationToken ct)
    {
        if (req.Items == null || !req.Items.Any()) return Result<Guid>.Failure("La venta no tiene ítems.");

        var venta = new Venta
        {
            VendedorId = req.VendedorId,
            ClienteId = req.ClienteId,
            FormaCobroId = req.FormaCobroId,
            EsContado = true
        };

        foreach (var item in req.Items)
        {
            var p = await _uow.Productos.GetByIdAsync(item.ProductoId, ct);
            if (p == null) continue;

            // 1. Ajustar Stock
            p.AjustarStock(item.Cantidad, TipoMovimientoStock.VENTA, $"Venta Directa: {venta.Id}");
            
            // 2. Agregar ítem a la venta
            venta.AgregarItem(p.Id, item.Cantidad, item.PrecioUnitario);

            // 3. Registrar Movimiento Auditoría
            var mov = new MovimientoStock
            {
                ProductoId = p.Id,
                Tipo = TipoMovimientoStock.VENTA,
                Cantidad = item.Cantidad,
                CostoUnitario = p.CostoUnitario,
                Motivo = $"Venta Contado ID: {venta.Id}"
            };
            await _uow.Productos.AddMovimientoAsync(mov, ct);
        }

        await _uow.Ventas.AddAsync(venta, ct);
        await _uow.SaveChangesAsync(ct);

        return Result<Guid>.Success(venta.Id);
    }
}

public record GetVentasRecientesQuery(int Cantidad = 100) : IRequest<Result<IEnumerable<object>>>;

public class GetVentasRecientesHandler : IRequestHandler<GetVentasRecientesQuery, Result<IEnumerable<object>>>
{
    private readonly IUnitOfWork _uow;
    public GetVentasRecientesHandler(IUnitOfWork uow) => _uow = uow;

    public async Task<Result<IEnumerable<object>>> Handle(GetVentasRecientesQuery req, CancellationToken ct)
    {
        var ventas = await _uow.Ventas.GetAllAsync(ct);
        var result = ventas.OrderByDescending(v => v.Fecha)
                           .Take(req.Cantidad)
                           .Select(v => new
                           {
                               v.Id,
                               v.Fecha,
                               v.Total,
                               FormaCobro = v.FormaCobro?.Nombre,
                               v.EsContado,
                               ClienteNombre = "Cliente Final" // Simplificación para el reporte
                           });

        return Result<IEnumerable<object>>.Success(result);
    }
}
