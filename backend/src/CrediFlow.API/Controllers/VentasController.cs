using CrediFlow.Application.Features.Logistica;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CrediFlow.API.Controllers;

[Authorize]
public class VentasController : BaseApiController
{
    private readonly IMediator _mediator;
    public VentasController(IMediator mediator) => _mediator = mediator;

    [HttpPost("contado")]
    public async Task<IActionResult> RegistrarVentaContado(RegistrarVentaContadoCommand command)
    {
        var result = await _mediator.Send(command);
        return result.IsSuccess ? Ok(new { id = result.Value }) : BadRequest(result.Error);
    }

    [HttpGet("recientes")]
    public async Task<IActionResult> GetRecientes()
    {
        var result = await _mediator.Send(new GetVentasRecientesQuery());
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }
}
