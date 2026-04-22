using CrediFlow.Application.Features.Reportes;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CrediFlow.API.Controllers;

[Authorize]
public class ReportesController : BaseApiController
{
    private readonly IMediator _mediator;

    public ReportesController(IMediator mediator) => _mediator = mediator;

    [HttpGet("deudores")]
    public async Task<IActionResult> GetDeudores(CancellationToken ct)
    {
        var result = await _mediator.Send(new GetDeudoresReporteQuery(), ct);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }
}
