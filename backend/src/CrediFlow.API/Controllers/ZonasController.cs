using CrediFlow.Application.Features.Zonas;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CrediFlow.API.Controllers;

[Authorize]
public class ZonasController : BaseApiController
{
    private readonly IMediator _mediator;

    public ZonasController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<ZonaDto>), 200)]
    public async Task<IActionResult> GetAll([FromQuery] bool soloActivas = true)
    {
        var result = await _mediator.Send(new GetZonasQuery(soloActivas));
        return Ok(result.Value);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CrearZonaCommand command)
    {
        var result = await _mediator.Send(command);
        if (!result.IsSuccess) return BadRequest(result.Error);
        return Ok(new { id = result.Value });
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] ActualizarZonaRequest request)
    {
        var result = await _mediator.Send(new ActualizarZonaCommand(id, request.Nombre, request.Descripcion, request.ColorHex));
        if (!result.IsSuccess) return BadRequest(result.Error);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await _mediator.Send(new DesactivarZonaCommand(id));
        if (!result.IsSuccess) return BadRequest(result.Error);
        return NoContent();
    }

    [HttpPost("{id}/reactivar")]
    public async Task<IActionResult> Reactivar(Guid id)
    {
        var result = await _mediator.Send(new ReactivarZonaCommand(id));
        if (!result.IsSuccess) return BadRequest(result.Error);
        return NoContent();
    }
}

public record ActualizarZonaRequest(string Nombre, string? Descripcion, string? ColorHex);
