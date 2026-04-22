using CrediFlow.Application.Common;
using CrediFlow.Application.DTOs;
using CrediFlow.Domain.Enums;
using CrediFlow.Domain.Interfaces;
using MediatR;

namespace CrediFlow.Application.Features.Clientes;

// ─── QUERIES ────────────────────────────────────────────────────────────────
public record GetClientesQuery(
    string? Search = null,
    EstadoCliente? Estado = null,
    Guid? ZonaId = null,
    Guid? TipoClienteId = null)
    : IRequest<Result<IEnumerable<ClienteResumenDto>>>;

public record GetClienteByIdQuery(Guid Id) : IRequest<Result<ClienteDto>>;

// ─── COMMANDS ───────────────────────────────────────────────────────────────
public record CrearClienteCommand(
    string NombreCompleto,
    string DNI,
    string Direccion,
    Guid ZonaId,
    string? CUIL = null,
    DateTime? FechaNacimiento = null,
    string? Sexo = null,
    Guid? TipoClienteId = null,
    Guid? OcupacionId = null,
    Guid? SectorId = null,
    string? Telefono = null,
    string? TelefonoAlternativo = null,
    string? Email = null,
    string? Barrio = null,
    string? Localidad = null,
    string? CodigoPostal = null,
    string? Provincia = null,
    double? Latitud = null,
    double? Longitud = null,
    decimal LimiteCredito = 0,
    Guid? ListaPrecioId = null,
    Guid? VendedorAsignadoId = null,
    string? FiadorNombre = null,
    string? FiadorDNI = null,
    string? FiadorTelefono = null,
    string? FiadorDireccion = null,
    string? EstadoCivil = null,
    string? Observaciones = null,
    string? FotoDNIFrente = null,
    string? FotoDNIDorso = null,
    string? FotoPerfil = null)
    : IRequest<Result<Guid>>;

public record ActualizarClienteCommand(
    Guid Id,
    string NombreCompleto,
    string Direccion,
    Guid ZonaId,
    string? CUIL = null,
    DateTime? FechaNacimiento = null,
    string? Sexo = null,
    Guid? TipoClienteId = null,
    Guid? OcupacionId = null,
    Guid? SectorId = null,
    string? Telefono = null,
    string? TelefonoAlternativo = null,
    string? Email = null,
    string? Barrio = null,
    string? Localidad = null,
    string? CodigoPostal = null,
    string? Provincia = null,
    double? Latitud = null,
    double? Longitud = null,
    decimal LimiteCredito = 0,
    Guid? ListaPrecioId = null,
    Guid? VendedorAsignadoId = null,
    string? FiadorNombre = null,
    string? FiadorDNI = null,
    string? FiadorTelefono = null,
    string? FiadorDireccion = null,
    string? EstadoCivil = null,
    string? Observaciones = null,
    string? FotoDNIFrente = null,
    string? FotoDNIDorso = null,
    string? FotoPerfil = null)
    : IRequest<Result>;

public record ReactivarClienteCommand(Guid Id) : IRequest<Result>;
public record InactivarClienteCommand(Guid Id, string? Motivo = null) : IRequest<Result>;
public record BloquearClienteCommand(Guid Id, string? Motivo = null) : IRequest<Result>;

// ─── HANDLERS ───────────────────────────────────────────────────────────────
public class GetClientesHandler : IRequestHandler<GetClientesQuery, Result<IEnumerable<ClienteResumenDto>>>
{
    private readonly IUnitOfWork _uow;
    public GetClientesHandler(IUnitOfWork uow) => _uow = uow;

    public async Task<Result<IEnumerable<ClienteResumenDto>>> Handle(
        GetClientesQuery request, CancellationToken ct)
    {
        var clientes = await _uow.Clientes.GetAllAsync(ct);

        if (request.ZonaId.HasValue)
            clientes = clientes.Where(c => c.ZonaId == request.ZonaId.Value);
        if (request.Estado.HasValue)
            clientes = clientes.Where(c => c.Estado == request.Estado.Value);
        if (request.TipoClienteId.HasValue)
            clientes = clientes.Where(c => c.TipoClienteId == request.TipoClienteId.Value);
        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var s = request.Search.ToLower();
            clientes = clientes.Where(c =>
                c.NombreCompleto.ToLower().Contains(s) ||
                c.DNI.Contains(s) ||
                (c.CUIL != null && c.CUIL.Contains(s)) ||
                (c.Telefono != null && c.Telefono.Contains(s)) ||
                (c.Localidad != null && c.Localidad.ToLower().Contains(s)));
        }

        var dtos = clientes.Select(c => new ClienteResumenDto(
            c.Id, c.NombreCompleto, c.DNI, c.CUIL,
            c.TipoClienteId, c.TipoCliente?.Nombre, c.Estado,
            c.Zona?.Nombre, c.Telefono, c.Localidad, c.LimiteCredito,
            c.TieneDeudaActiva));

        return Result<IEnumerable<ClienteResumenDto>>.Success(dtos);
    }
}

public class GetClienteByIdHandler : IRequestHandler<GetClienteByIdQuery, Result<ClienteDto>>
{
    private readonly IUnitOfWork _uow;
    public GetClienteByIdHandler(IUnitOfWork uow) => _uow = uow;

    public async Task<Result<ClienteDto>> Handle(GetClienteByIdQuery request, CancellationToken ct)
    {
        var c = await _uow.Clientes.GetByIdAsync(request.Id, ct);
        if (c is null) return Result<ClienteDto>.Failure("Cliente no encontrado.");

        var dto = new ClienteDto(
            c.Id, c.NombreCompleto, c.DNI, c.CUIL,
            c.FechaNacimiento, c.Sexo, c.EstadoCivil,
            c.OcupacionId, c.Ocupacion?.Nombre,
            c.SectorId, c.Sector?.Nombre,
            c.TipoClienteId, c.TipoCliente?.Nombre,
            c.Direccion,
            c.Barrio, c.Localidad, c.CodigoPostal, c.Provincia,
            c.Telefono, c.TelefonoAlternativo, c.Email,
            c.ZonaId, c.Zona?.Nombre, c.VendedorAsignadoId,
            c.Estado, c.Latitud, c.Longitud,
            c.LimiteCredito, c.ListaPrecioId,
            c.FiadorNombre, c.FiadorDNI, c.FiadorTelefono, c.FiadorDireccion,
            c.FotoPerfil, c.FotoDNIFrente, c.FotoDNIDorso,
            c.Observaciones, c.FechaAlta, c.TieneDeudaActiva, c.Edad);

        return Result<ClienteDto>.Success(dto);
    }
}

public class CrearClienteHandler : IRequestHandler<CrearClienteCommand, Result<Guid>>
{
    private readonly IUnitOfWork _uow;
    private readonly ICurrentUserService _currentUser;

    public CrearClienteHandler(IUnitOfWork uow, ICurrentUserService currentUser)
    {
        _uow = uow;
        _currentUser = currentUser;
    }

    public async Task<Result<Guid>> Handle(CrearClienteCommand req, CancellationToken ct)
    {
        var existente = await _uow.Clientes.GetByDniAsync(req.DNI, ct);
        if (existente != null) return Result<Guid>.Failure($"Ya existe un cliente con DNI {req.DNI}.");

        var zona = await _uow.Zonas.GetByIdAsync(req.ZonaId, ct);
        if (zona is null) return Result<Guid>.Failure("Zona no encontrada.");

        var cliente = Domain.Entities.Cliente.Crear(
            req.NombreCompleto, req.DNI, req.Direccion, req.ZonaId,
            req.CUIL, req.FechaNacimiento, req.Sexo,
            req.TipoClienteId, req.OcupacionId, req.SectorId,
            req.Telefono, req.TelefonoAlternativo, req.Email,
            req.Barrio, req.Localidad, req.CodigoPostal, req.Provincia,
            req.Latitud, req.Longitud,
            req.LimiteCredito, req.ListaPrecioId, req.VendedorAsignadoId,
            req.FiadorNombre, req.FiadorDNI, req.FiadorTelefono, req.FiadorDireccion,
            req.EstadoCivil, req.Observaciones);

        cliente.ActualizarFotos(req.FotoDNIFrente, req.FotoDNIDorso, req.FotoPerfil);
        cliente.SetAudit(_currentUser.UserId.ToString()!);
        await _uow.Clientes.AddAsync(cliente, ct);
        await _uow.SaveChangesAsync(ct);

        return Result<Guid>.Success(cliente.Id);
    }
}

public class ActualizarClienteHandler : IRequestHandler<ActualizarClienteCommand, Result>
{
    private readonly IUnitOfWork _uow;
    private readonly ICurrentUserService _currentUser;

    public ActualizarClienteHandler(IUnitOfWork uow, ICurrentUserService currentUser)
    {
        _uow = uow;
        _currentUser = currentUser;
    }

    public async Task<Result> Handle(ActualizarClienteCommand req, CancellationToken ct)
    {
        var cliente = await _uow.Clientes.GetByIdAsync(req.Id, ct);
        if (cliente is null) return Result.Failure("Cliente no encontrado.");

        cliente.Actualizar(
            req.NombreCompleto, req.Direccion, req.ZonaId,
            req.CUIL, req.FechaNacimiento, req.Sexo,
            req.TipoClienteId, req.OcupacionId, req.SectorId,
            req.Telefono, req.TelefonoAlternativo, req.Email,
            req.Barrio, req.Localidad, req.CodigoPostal, req.Provincia,
            req.Latitud, req.Longitud,
            req.LimiteCredito, req.ListaPrecioId, req.VendedorAsignadoId,
            req.FiadorNombre, req.FiadorDNI, req.FiadorTelefono, req.FiadorDireccion,
            req.EstadoCivil, req.Observaciones);

        cliente.ActualizarFotos(req.FotoDNIFrente, req.FotoDNIDorso, req.FotoPerfil);
        cliente.SetAudit(_currentUser.UserId.ToString()!, isUpdate: true);

        _uow.Clientes.Update(cliente);
        await _uow.SaveChangesAsync(ct);
        return Result.Success();
    }
}

public class ReactivarClienteHandler : IRequestHandler<ReactivarClienteCommand, Result>
{
    private readonly IUnitOfWork _uow;
    public ReactivarClienteHandler(IUnitOfWork uow) => _uow = uow;

    public async Task<Result> Handle(ReactivarClienteCommand request, CancellationToken ct)
    {
        var cliente = await _uow.Clientes.GetByIdAsync(request.Id, ct);
        if (cliente is null) return Result.Failure("Cliente no encontrado.");
        cliente.CambiarEstado(EstadoCliente.ACTIVO);
        await _uow.SaveChangesAsync(ct);
        return Result.Success();
    }
}

public class InactivarClienteHandler : IRequestHandler<InactivarClienteCommand, Result>
{
    private readonly IUnitOfWork _uow;
    public InactivarClienteHandler(IUnitOfWork uow) => _uow = uow;

    public async Task<Result> Handle(InactivarClienteCommand request, CancellationToken ct)
    {
        var cliente = await _uow.Clientes.GetByIdAsync(request.Id, ct);
        if (cliente is null) return Result.Failure("Cliente no encontrado.");
        if (cliente.TieneDeudaActiva) return Result.Failure("No se puede inactivar un cliente con deuda activa.");
        cliente.CambiarEstado(EstadoCliente.INACTIVO);
        await _uow.SaveChangesAsync(ct);
        return Result.Success();
    }
}

public class BloquearClienteHandler : IRequestHandler<BloquearClienteCommand, Result>
{
    private readonly IUnitOfWork _uow;
    public BloquearClienteHandler(IUnitOfWork uow) => _uow = uow;

    public async Task<Result> Handle(BloquearClienteCommand request, CancellationToken ct)
    {
        var cliente = await _uow.Clientes.GetByIdAsync(request.Id, ct);
        if (cliente is null) return Result.Failure("Cliente no encontrado.");
        cliente.CambiarEstado(EstadoCliente.BLOQUEADO);
        await _uow.SaveChangesAsync(ct);
        return Result.Success();
    }
}
