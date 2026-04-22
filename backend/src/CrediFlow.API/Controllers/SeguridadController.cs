using CrediFlow.Application.Features.Seguridad;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CrediFlow.API.Controllers;

public class SeguridadController : BaseApiController
{
    private readonly IMediator _mediator;
    public SeguridadController(IMediator mediator) => _mediator = mediator;

    [HttpGet("perfiles")]
    public async Task<IActionResult> GetPerfiles()
    {
        var result = await _mediator.Send(new GetPerfilesQuery());
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    [HttpGet("permisos")]
    public async Task<IActionResult> GetPermisos()
    {
        var result = await _mediator.Send(new GetPermisosQuery());
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    [HttpPost("perfiles")]
    public async Task<IActionResult> CrearPerfil(CrearPerfilCommand command)
    {
        var result = await _mediator.Send(command);
        return result.IsSuccess ? Ok(new { id = result.Value }) : BadRequest(result.Error);
    }

    [HttpGet("usuarios")]
    public async Task<IActionResult> GetUsuarios()
    {
        var result = await _mediator.Send(new GetUsuariosQuery());
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    [HttpPut("usuarios/{id}")]
    public async Task<IActionResult> ActualizarUsuario(Guid id, ActualizarUsuarioCommand command)
    {
        if (id != command.Id) return BadRequest("ID no coincide");
        var result = await _mediator.Send(command);
        return result.IsSuccess ? NoContent() : BadRequest(result.Error);
    }

    [HttpPost("usuarios/{id}/reactivar")]
    public async Task<IActionResult> ReactivarUsuario(Guid id)
    {
        var result = await _mediator.Send(new ReactivarUsuarioCommand(id));
        return result.IsSuccess ? NoContent() : BadRequest(result.Error);
    }

    [HttpPost("perfiles/{id}/reactivar")]
    public async Task<IActionResult> ReactivarPerfil(Guid id)
    {
        var result = await _mediator.Send(new ReactivarPerfilCommand(id));
        return result.IsSuccess ? NoContent() : BadRequest(result.Error);
    }
}
