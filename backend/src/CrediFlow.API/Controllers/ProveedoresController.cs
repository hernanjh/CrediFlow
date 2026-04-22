using CrediFlow.Application.Features.Proveedores;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CrediFlow.API.Controllers;

public class ProveedoresController : BaseApiController
{
    private readonly IMediator _mediator;
    public ProveedoresController(IMediator mediator) => _mediator = mediator;

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] bool soloActivos = true)
    {
        var result = await _mediator.Send(new GetProveedoresQuery(soloActivos));
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CrearProveedorCommand command)
    {
        var result = await _mediator.Send(command);
        return result.IsSuccess ? Ok(new { id = result.Value }) : BadRequest(result.Error);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, ActualizarProveedorCommand command)
    {
        if (id != command.Id) return BadRequest("ID no coincide");
        var result = await _mediator.Send(command);
        return result.IsSuccess ? NoContent() : BadRequest(result.Error);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Desactivar(Guid id)
    {
        var result = await _mediator.Send(new DesactivarProveedorCommand(id));
        return result.IsSuccess ? NoContent() : BadRequest(result.Error);
    }

    [HttpPost("{id}/reactivar")]
    public async Task<IActionResult> Reactivar(Guid id)
    {
        var result = await _mediator.Send(new ReactivarProveedorCommand(id));
        return result.IsSuccess ? NoContent() : BadRequest(result.Error);
    }
}
