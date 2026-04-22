using CrediFlow.Application.Features.Inventario;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CrediFlow.API.Controllers;

[Authorize]
public class InventarioController : BaseApiController
{
    private readonly IMediator _mediator;

    public InventarioController(IMediator mediator) => _mediator = mediator;

    [HttpGet("productos")]
    [ProducesResponseType(typeof(IEnumerable<CrediFlow.Application.DTOs.ProductoDto>), 200)]
    public async Task<IActionResult> GetProductos(
        [FromQuery] string? search,
        [FromQuery] bool soloActivos = false,
        CancellationToken ct = default)
    {
        var result = await _mediator.Send(new GetProductosQuery(search, soloActivos), ct);
        return HandleResult(result);
    }

    [HttpGet("productos/{id:guid}")]
    public async Task<IActionResult> GetProductoById(Guid id, CancellationToken ct)
    {
        var result = await _mediator.Send(new GetProductoByIdQuery(id), ct);
        return HandleResult(result);
    }

    [HttpPost("productos")]
    public async Task<IActionResult> CrearProducto([FromBody] CrearProductoCommand command, CancellationToken ct)
    {
        var result = await _mediator.Send(command, ct);
        if (!result.IsSuccess) return BadRequest(result.Error);
        return CreatedAtAction(nameof(GetProductoById), new { id = result.Value }, result.Value);
    }

    [HttpPut("productos/{id:guid}")]
    public async Task<IActionResult> ActualizarProducto(Guid id, [FromBody] ActualizarProductoRequest request, CancellationToken ct)
    {
        var command = new ActualizarProductoCommand(
            id, request.Nombre, request.Descripcion,
            request.CostoUnitario, request.PrecioVenta, request.StockMinimo,
            request.CategoriaId, request.ProveedorId, request.Foto,
            request.CodigoInterno, request.SKU,
            request.PrecioVentaMayorista, request.StockMaximo,
            request.UnidadMedidaId, request.PermiteVentaSinStock);

        var result = await _mediator.Send(command, ct);
        return HandleResult(result);
    }

    [HttpDelete("productos/{id:guid}")]
    public async Task<IActionResult> EliminarProducto(Guid id, CancellationToken ct)
    {
        var result = await _mediator.Send(new EliminarProductoCommand(id), ct);
        if (!result.IsSuccess) return BadRequest(result.Error);
        return NoContent();
    }

    [HttpPost("productos/{id:guid}/reactivar")]
    public async Task<IActionResult> ReactivarProducto(Guid id, CancellationToken ct)
    {
        var result = await _mediator.Send(new ReactivarProductoCommand(id), ct);
        if (!result.IsSuccess) return BadRequest(result.Error);
        return NoContent();
    }

    [HttpPost("compras/factura")]
    public async Task<IActionResult> CargarFacturaCompra([FromBody] CargarFacturaCompraCommand command, CancellationToken ct)
    {
        var result = await _mediator.Send(command, ct);
        if (!result.IsSuccess) return BadRequest(result.Error);
        return Ok(new { message = "Factura procesada. Stock actualizado." });
    }
}

public record ActualizarProductoRequest(
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
    decimal? PrecioVentaMayorista,
    int? StockMaximo,
    Guid? UnidadMedidaId,
    bool PermiteVentaSinStock);
