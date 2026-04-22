using CrediFlow.Application.Common;
using CrediFlow.Application.DTOs;
using CrediFlow.Domain.Entities;
using CrediFlow.Domain.Interfaces;
using MediatR;

namespace CrediFlow.Application.Features.Zonas;

// ─── QUERIES ─────────────────────────────────────────────────────────────
public record GetZonasQuery(bool SoloActivas = true) : IRequest<Result<IEnumerable<ZonaDto>>>;

public record ZonaDto(Guid Id, string Nombre, string? Descripcion, string? ColorHex, int CantidadClientes, bool Activa);

public class GetZonasHandler : IRequestHandler<GetZonasQuery, Result<IEnumerable<ZonaDto>>>
{
    private readonly IUnitOfWork _uow;
    public GetZonasHandler(IUnitOfWork uow) => _uow = uow;

    public async Task<Result<IEnumerable<ZonaDto>>> Handle(GetZonasQuery request, CancellationToken ct)
    {
        var zonas = await _uow.Zonas.GetAllAsync(ct);
        if (request.SoloActivas) zonas = zonas.Where(z => z.Activa);

        var dtos = zonas.Select(z => new ZonaDto(z.Id, z.Nombre, z.Descripcion, z.ColorHex, z.Clientes?.Count ?? 0, z.Activa));
        return Result<IEnumerable<ZonaDto>>.Success(dtos);
    }
}

// ─── COMMANDS ────────────────────────────────────────────────────────────
public record CrearZonaCommand(string Nombre, string? Descripcion, string? ColorHex) : IRequest<Result<Guid>>;

public class CrearZonaHandler : IRequestHandler<CrearZonaCommand, Result<Guid>>
{
    private readonly IUnitOfWork _uow;
    public CrearZonaHandler(IUnitOfWork uow) => _uow = uow;

    public async Task<Result<Guid>> Handle(CrearZonaCommand req, CancellationToken ct)
    {
        var z = Zona.Crear(req.Nombre, req.Descripcion, null, req.ColorHex);
        await _uow.Zonas.AddAsync(z, ct);
        await _uow.SaveChangesAsync(ct);
        return Result<Guid>.Success(z.Id);
    }
}

public record ActualizarZonaCommand(Guid Id, string Nombre, string? Descripcion, string? ColorHex) : IRequest<Result<Unit>>;

public class ActualizarZonaHandler : IRequestHandler<ActualizarZonaCommand, Result<Unit>>
{
    private readonly IUnitOfWork _uow;
    public ActualizarZonaHandler(IUnitOfWork uow) => _uow = uow;

    public async Task<Result<Unit>> Handle(ActualizarZonaCommand req, CancellationToken ct)
    {
        var z = await _uow.Zonas.GetByIdAsync(req.Id, ct);
        if (z == null) return Result<Unit>.Failure("Zona no encontrada");
        
        z.Actualizar(req.Nombre, req.Descripcion, z.CoordenadasGeoJson, req.ColorHex);

        await _uow.SaveChangesAsync(ct);
        return Result<Unit>.Success(Unit.Value);
    }
}

public record DesactivarZonaCommand(Guid Id) : IRequest<Result<Unit>>;

public class DesactivarZonaHandler : IRequestHandler<DesactivarZonaCommand, Result<Unit>>
{
    private readonly IUnitOfWork _uow;
    public DesactivarZonaHandler(IUnitOfWork uow) => _uow = uow;

    public async Task<Result<Unit>> Handle(DesactivarZonaCommand req, CancellationToken ct)
    {
        var z = await _uow.Zonas.GetByIdAsync(req.Id, ct);
        if (z == null) return Result<Unit>.Failure("Zona no encontrada");

        z.Desactivar();
        await _uow.SaveChangesAsync(ct);
        return Result<Unit>.Success(Unit.Value);
    }
}

public record ReactivarZonaCommand(Guid Id) : IRequest<Result<Unit>>;

public class ReactivarZonaHandler : IRequestHandler<ReactivarZonaCommand, Result<Unit>>
{
    private readonly IUnitOfWork _uow;
    public ReactivarZonaHandler(IUnitOfWork uow) => _uow = uow;

    public async Task<Result<Unit>> Handle(ReactivarZonaCommand req, CancellationToken ct)
    {
        var z = await _uow.Zonas.GetByIdAsync(req.Id, ct);
        if (z == null) return Result<Unit>.Failure("Zona no encontrada");

        z.Activar();
        await _uow.SaveChangesAsync(ct);
        return Result<Unit>.Success(Unit.Value);
    }
}
