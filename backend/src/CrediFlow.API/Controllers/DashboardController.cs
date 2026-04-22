using CrediFlow.Application.Features.Dashboard;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CrediFlow.API.Controllers;

[Authorize]
public class DashboardController : BaseApiController
{
    private readonly IMediator _mediator;
    public DashboardController(IMediator mediator) => _mediator = mediator;

    [HttpGet("kpi")]
    public async Task<IActionResult> GetKpi(CancellationToken ct)
        => HandleResult(await _mediator.Send(new GetDashboardKpiQuery(), ct));

    [HttpGet("cobranza-termometro")]
    public async Task<IActionResult> GetTermometro(CancellationToken ct)
        => HandleResult(await _mediator.Send(new GetCobranzaTermometroQuery(), ct));

    [HttpGet("evolucion-mensual")]
    public async Task<IActionResult> GetEvolucionMensual([FromQuery] int meses = 6, CancellationToken ct = default)
        => HandleResult(await _mediator.Send(new GetEvolucionMensualQuery(meses), ct));

    [HttpGet("distribucion-cartera")]
    public async Task<IActionResult> GetDistribucionCartera(CancellationToken ct)
        => HandleResult(await _mediator.Send(new GetDistribucionCarteraQuery(), ct));

    [HttpGet("ranking-vendedores")]
    [Authorize(Roles = "SUPER_ADMIN,ADMIN")]
    public async Task<IActionResult> GetRankingVendedores(
        [FromQuery] DateTime? desde, [FromQuery] DateTime? hasta, CancellationToken ct)
        => HandleResult(await _mediator.Send(new GetRankingVendedoresQuery(desde, hasta), ct));

    [HttpGet("aging-deuda")]
    [Authorize(Roles = "SUPER_ADMIN,ADMIN")]
    public async Task<IActionResult> GetAgingDeuda(CancellationToken ct)
        => HandleResult(await _mediator.Send(new GetAgingDeudaQuery(), ct));

    [HttpGet("flujo-proyectado")]
    [Authorize(Roles = "SUPER_ADMIN,ADMIN")]
    public async Task<IActionResult> GetFlujoProyectado(CancellationToken ct)
        => HandleResult(await _mediator.Send(new GetFlujoCajaProyectadoQuery(), ct));

    [HttpGet("mapa-mora")]
    [Authorize(Roles = "SUPER_ADMIN,ADMIN")]
    public async Task<IActionResult> GetMapaMora(CancellationToken ct)
        => HandleResult(await _mediator.Send(new GetMapaMoraQuery(), ct));

    [HttpGet("alertas")]
    public async Task<IActionResult> GetAlertas(CancellationToken ct)
        => HandleResult(await _mediator.Send(new GetAlertasQuery(), ct));
}
