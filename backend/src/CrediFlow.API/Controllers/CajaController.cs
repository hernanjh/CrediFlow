using CrediFlow.Application.Features.Caja;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CrediFlow.API.Controllers;

[Authorize]
public class CajaController : BaseApiController
{
    private readonly IMediator _mediator;
    public CajaController(IMediator mediator) => _mediator = mediator;

    /// <summary>
    /// CIERRE CIEGO - Paso 1: Vendedor declara su monto sin ver el total del sistema
    /// </summary>
    [HttpPost("declarar")]
    [Authorize(Roles = "VENDEDOR,ADMIN,SUPER_ADMIN")]
    public async Task<IActionResult> Declarar([FromBody] DeclararCajaCommand command, CancellationToken ct)
    {
        var result = await _mediator.Send(command, ct);
        return HandleResult(result);
    }

    /// <summary>
    /// CIERRE CIEGO - Paso 2 (SOLO SuperAdmin/Admin): cierra comparando con el sistema
    /// </summary>
    [HttpPost("{vendedorId:guid}/cerrar")]
    [Authorize(Roles = "SUPER_ADMIN,ADMIN")]
    public async Task<IActionResult> CerrarConSistema(Guid vendedorId, [FromQuery] DateTime? fecha, CancellationToken ct)
    {
        var f = fecha ?? DateTime.UtcNow.Date;
        var result = await _mediator.Send(new CerrarCajaConSistemaCommand(vendedorId, f), ct);
        return HandleResult(result);
    }

    /// <summary>
    /// Ver cierres de caja del día (SOLO SuperAdmin/Admin) — el vendedor NO puede llamar esto
    /// </summary>
    [HttpGet("hoy")]
    [Authorize(Roles = "SUPER_ADMIN,ADMIN")]
    public async Task<IActionResult> GetHoy(CancellationToken ct)
    {
        var result = await _mediator.Send(new GetCierresCajaHoyQuery(), ct);
        return HandleResult(result);
    }
}
