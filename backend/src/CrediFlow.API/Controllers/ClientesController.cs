using CrediFlow.Application.Features.Clientes;
using CrediFlow.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CrediFlow.API.Controllers;

[Authorize]
public class ClientesController : BaseApiController
{
    private readonly IMediator _mediator;

    public ClientesController(IMediator mediator) => _mediator = mediator;

    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] string? search,
        [FromQuery] EstadoCliente? estado,
        [FromQuery] Guid? zonaId,
        [FromQuery] Guid? tipoClienteId,
        CancellationToken ct)
    {
        var result = await _mediator.Send(new GetClientesQuery(search, estado, zonaId, tipoClienteId), ct);
        return HandleResult(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var result = await _mediator.Send(new GetClienteByIdQuery(id), ct);
        return HandleResult(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CrearClienteCommand command, CancellationToken ct)
    {
        var result = await _mediator.Send(command, ct);
        if (!result.IsSuccess) return HandleResult(result);
        return CreatedAtAction(nameof(GetById), new { id = result.Value }, result.Value);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] ActualizarClienteRequest request, CancellationToken ct)
    {
        var command = new ActualizarClienteCommand(
            id,
            request.NombreCompleto,
            request.Direccion,
            request.ZonaId,
            request.CUIL,
            request.FechaNacimiento,
            request.Sexo,
            request.TipoClienteId,
            request.OcupacionId,
            request.SectorId,
            request.Telefono,
            request.TelefonoAlternativo,
            request.Email,
            request.Barrio,
            request.Localidad,
            request.CodigoPostal,
            request.Provincia,
            request.Latitud,
            request.Longitud,
            request.LimiteCredito,
            request.ListaPrecioId,
            request.VendedorAsignadoId,
            request.FiadorNombre,
            request.FiadorDNI,
            request.FiadorTelefono,
            request.FiadorDireccion,
            request.EstadoCivil,
            request.Observaciones,
            request.FotoDNIFrente,
            request.FotoDNIDorso,
            request.FotoPerfil);

        var result = await _mediator.Send(command, ct);
        return HandleResult(result);
    }

    [HttpPost("{id:guid}/reactivar")]
    public async Task<IActionResult> Reactivar(Guid id, CancellationToken ct)
    {
        var result = await _mediator.Send(new ReactivarClienteCommand(id), ct);
        return HandleResult(result);
    }

    [HttpPost("{id:guid}/inactivar")]
    public async Task<IActionResult> Inactivar(Guid id, CancellationToken ct)
    {
        var result = await _mediator.Send(new InactivarClienteCommand(id), ct);
        return HandleResult(result);
    }

    [HttpPost("{id:guid}/bloquear")]
    public async Task<IActionResult> Bloquear(Guid id, CancellationToken ct)
    {
        var result = await _mediator.Send(new BloquearClienteCommand(id), ct);
        return HandleResult(result);
    }
}

public record ActualizarClienteRequest(
    string NombreCompleto,
    string Direccion,
    Guid ZonaId,
    string? CUIL,
    DateTime? FechaNacimiento,
    string? Sexo,
    Guid? TipoClienteId,
    Guid? OcupacionId,
    Guid? SectorId,
    string? Telefono,
    string? TelefonoAlternativo,
    string? Email,
    string? Barrio,
    string? Localidad,
    string? CodigoPostal,
    string? Provincia,
    double? Latitud,
    double? Longitud,
    decimal LimiteCredito,
    Guid? ListaPrecioId,
    Guid? VendedorAsignadoId,
    string? FiadorNombre,
    string? FiadorDNI,
    string? FiadorTelefono,
    string? FiadorDireccion,
    string? EstadoCivil,
    string? Observaciones,
    string? FotoDNIFrente,
    string? FotoDNIDorso,
    string? FotoPerfil);
