using CrediFlow.Application.Features.Configuracion;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CrediFlow.API.Controllers;

public class ConfiguracionController : BaseApiController
{
    private readonly IMediator _mediator;
    public ConfiguracionController(IMediator mediator) => _mediator = mediator;

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var result = await _mediator.Send(new GetConfiguracionQuery());
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    [HttpPut]
    public async Task<IActionResult> Update(UpdateConfiguracionCommand command)
    {
        var result = await _mediator.Send(command);
        return result.IsSuccess ? NoContent() : BadRequest(result.Error);
    }

    [HttpGet("listas-precios")]
    public async Task<IActionResult> GetListasPrecios()
    {
        var result = await _mediator.Send(new GetListasPreciosQuery());
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    [HttpPost("listas-precios")]
    public async Task<IActionResult> CrearListaPrecio(CrearListaPrecioCommand command)
    {
        var result = await _mediator.Send(command);
        return result.IsSuccess ? Ok(new { id = result.Value }) : BadRequest(result.Error);
    }

    [HttpPost("listas-precios/{id}/reactivar")]
    public async Task<IActionResult> ReactivarListaPrecio(Guid id)
    {
        var result = await _mediator.Send(new ReactivarListaPrecioCommand(id));
        return result.IsSuccess ? NoContent() : BadRequest(result.Error);
    }

    [HttpPut("listas-precios/{id}")]
    public async Task<IActionResult> ActualizarListaPrecio(Guid id, ActualizarListaPrecioCommand command)
    {
        if (id != command.Id) return BadRequest("ID no coincide");
        var result = await _mediator.Send(command);
        return result.IsSuccess ? NoContent() : BadRequest(result.Error);
    }

    [HttpPost("listas-precios/{id}/items")]
    public async Task<IActionResult> AgregarItemListaPrecio(Guid id, AgregarItemListaPrecioCommand command)
    {
        if (id != command.ListaPrecioId) return BadRequest("ID de lista no coincide");
        var result = await _mediator.Send(command);
        return result.IsSuccess ? Ok(new { id = result.Value }) : BadRequest(result.Error);
    }

    [HttpDelete("listas-precios/items/{itemId}")]
    public async Task<IActionResult> EliminarItemListaPrecio(Guid itemId)
    {
        var result = await _mediator.Send(new EliminarItemListaPrecioCommand(itemId));
        return result.IsSuccess ? NoContent() : BadRequest(result.Error);
    }
}
