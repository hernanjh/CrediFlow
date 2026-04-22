using CrediFlow.Application.Features.Creditos;
using CrediFlow.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CrediFlow.API.Controllers;

[Authorize]
public class CreditosController : BaseApiController
{
    private readonly IMediator _mediator;
    public CreditosController(IMediator mediator) => _mediator = mediator;

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var result = await _mediator.Send(new GetCreditoByIdQuery(id), ct);
        return HandleResult(result);
    }

    [HttpGet("cliente/{clienteId:guid}")]
    public async Task<IActionResult> GetByCliente(Guid clienteId, CancellationToken ct)
    {
        var result = await _mediator.Send(new GetCreditosByClienteQuery(clienteId), ct);
        return HandleResult(result);
    }

    [HttpPost]
    [Authorize(Roles = "SUPER_ADMIN,ADMIN,VENDEDOR")]
    public async Task<IActionResult> Create([FromBody] CrearCreditoCommand command, CancellationToken ct)
    {
        var result = await _mediator.Send(command, ct);
        if (!result.IsSuccess) return HandleResult(result);
        return CreatedAtAction(nameof(GetById), new { id = result.Value }, result.Value);
    }

    [HttpPost("{cuotaId:guid}/pagar")]
    [Authorize(Roles = "SUPER_ADMIN,ADMIN,VENDEDOR")]
    public async Task<IActionResult> RegistrarPago(Guid cuotaId, [FromBody] PagoRequest request, CancellationToken ct)
    {
        var command = new RegistrarPagoCommand(
            cuotaId, request.Monto, request.Latitud, request.Longitud,
            request.Observaciones, request.EsOffline, request.DeviceId);
        var result = await _mediator.Send(command, ct);
        return HandleResult(result);
    }

    [HttpGet("hoja-ruta")]
    [Authorize(Roles = "SUPER_ADMIN,ADMIN,VENDEDOR")]
    public async Task<IActionResult> GetHojaRuta([FromQuery] Guid vendedorId, [FromQuery] DateTime? fecha, CancellationToken ct)
    {
        var result = await _mediator.Send(new GetHojaRutaQuery(vendedorId, fecha), ct);
        return HandleResult(result);
    }

    [HttpPost("sync-offline")]
    [Authorize(Roles = "SUPER_ADMIN,ADMIN,VENDEDOR")]
    public async Task<IActionResult> SyncOffline([FromBody] SyncPagosOfflineCommand command, CancellationToken ct)
    {
        var result = await _mediator.Send(command, ct);
        return HandleResult(result);
    }
}

public record PagoRequest(
    decimal Monto, double? Latitud, double? Longitud,
    string? Observaciones, bool EsOffline = false, string? DeviceId = null);
