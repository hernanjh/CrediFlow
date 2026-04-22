using CrediFlow.Application.Features.Catalogos;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CrediFlow.API.Controllers;

public class CategoriasController : BaseApiController
{
    private readonly IMediator _mediator;
    public CategoriasController(IMediator mediator) => _mediator = mediator;

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] bool soloActivas = true)
    {
        var result = await _mediator.Send(new GetCategoriasQuery(soloActivas));
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CrearCategoriaCommand command)
    {
        var result = await _mediator.Send(command);
        return result.IsSuccess ? Ok(new { id = result.Value }) : BadRequest(result.Error);
    }
}
