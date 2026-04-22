using CrediFlow.Application.Common;
using CrediFlow.Application.DTOs;
using CrediFlow.Domain.Entities;
using CrediFlow.Domain.Interfaces;
using MediatR;

namespace CrediFlow.Application.Features.Proveedores;

// ─── QUERIES ─────────────────────────────────────────────────────────────
public record GetProveedoresQuery(bool SoloActivos = false, string? Search = null)
    : IRequest<Result<IEnumerable<ProveedorDto>>>;

public class GetProveedoresHandler : IRequestHandler<GetProveedoresQuery, Result<IEnumerable<ProveedorDto>>>
{
    private readonly IUnitOfWork _uow;
    public GetProveedoresHandler(IUnitOfWork uow) => _uow = uow;

    public async Task<Result<IEnumerable<ProveedorDto>>> Handle(GetProveedoresQuery request, CancellationToken ct)
    {
        var provs = await _uow.Proveedores.GetAllAsync(ct);

        if (request.SoloActivos)
            provs = provs.Where(p => p.Activo);

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var s = request.Search.ToLower();
            provs = provs.Where(p =>
                p.RazonSocial.ToLower().Contains(s) ||
                (p.CUIT != null && p.CUIT.Contains(s)) ||
                (p.NombreFantasia != null && p.NombreFantasia.ToLower().Contains(s)));
        }

        var dtos = provs.Select(ToDto);
        return Result<IEnumerable<ProveedorDto>>.Success(dtos);
    }

    private static ProveedorDto ToDto(Proveedor p) => new ProveedorDto(
        p.Id, p.RazonSocial, p.NombreFantasia, p.CUIT,
        p.ContactoPrincipal, p.Telefono, p.Celular,
        p.Email, p.Web,
        p.Direccion, p.Localidad, p.Provincia,
        p.CondicionPago, p.CondicionIVA, p.Notas,
        p.Activo);
}

// ─── COMMANDS ────────────────────────────────────────────────────────────
public record CrearProveedorCommand(
    string RazonSocial,
    string? Cuit,
    string? Telefono,
    string? Email,
    string? Direccion,
    string? NombreFantasia = null,
    string? ContactoPrincipal = null,
    string? Celular = null,
    string? Web = null,
    string? Localidad = null,
    string? Provincia = null,
    string? CondicionPago = null,
    string? CondicionIVA = null,
    string? Notas = null)
    : IRequest<Result<Guid>>;

public class CrearProveedorHandler : IRequestHandler<CrearProveedorCommand, Result<Guid>>
{
    private readonly IUnitOfWork _uow;
    public CrearProveedorHandler(IUnitOfWork uow) => _uow = uow;

    public async Task<Result<Guid>> Handle(CrearProveedorCommand req, CancellationToken ct)
    {
        var p = Proveedor.Crear(
            req.RazonSocial, req.Cuit, req.Telefono, req.Email, req.Direccion,
            req.NombreFantasia, req.ContactoPrincipal, req.Celular,
            req.Web, req.Localidad, req.Provincia,
            req.CondicionPago, req.CondicionIVA, req.Notas);

        await _uow.Proveedores.AddAsync(p, ct);
        await _uow.SaveChangesAsync(ct);
        return Result<Guid>.Success(p.Id);
    }
}

public record ActualizarProveedorCommand(
    Guid Id,
    string RazonSocial,
    string? Cuit,
    string? Telefono,
    string? Email,
    string? Direccion,
    string? NombreFantasia = null,
    string? ContactoPrincipal = null,
    string? Celular = null,
    string? Web = null,
    string? Localidad = null,
    string? Provincia = null,
    string? CondicionPago = null,
    string? CondicionIVA = null,
    string? Notas = null)
    : IRequest<Result<Unit>>;

public class ActualizarProveedorHandler : IRequestHandler<ActualizarProveedorCommand, Result<Unit>>
{
    private readonly IUnitOfWork _uow;
    public ActualizarProveedorHandler(IUnitOfWork uow) => _uow = uow;

    public async Task<Result<Unit>> Handle(ActualizarProveedorCommand req, CancellationToken ct)
    {
        var p = await _uow.Proveedores.GetByIdAsync(req.Id, ct);
        if (p == null) return Result<Unit>.Failure("Proveedor no encontrado");

        p.Actualizar(
            req.RazonSocial, req.Cuit, req.Telefono, req.Email, req.Direccion,
            req.NombreFantasia, req.ContactoPrincipal, req.Celular,
            req.Web, req.Localidad, req.Provincia,
            req.CondicionPago, req.CondicionIVA, req.Notas);

        await _uow.SaveChangesAsync(ct);
        return Result<Unit>.Success(Unit.Value);
    }
}

public record DesactivarProveedorCommand(Guid Id) : IRequest<Result<Unit>>;

public class DesactivarProveedorHandler : IRequestHandler<DesactivarProveedorCommand, Result<Unit>>
{
    private readonly IUnitOfWork _uow;
    public DesactivarProveedorHandler(IUnitOfWork uow) => _uow = uow;

    public async Task<Result<Unit>> Handle(DesactivarProveedorCommand req, CancellationToken ct)
    {
        var p = await _uow.Proveedores.GetByIdAsync(req.Id, ct);
        if (p == null) return Result<Unit>.Failure("Proveedor no encontrado");
        p.Desactivar();
        await _uow.SaveChangesAsync(ct);
        return Result<Unit>.Success(Unit.Value);
    }
}

public record ReactivarProveedorCommand(Guid Id) : IRequest<Result<Unit>>;

public class ReactivarProveedorHandler : IRequestHandler<ReactivarProveedorCommand, Result<Unit>>
{
    private readonly IUnitOfWork _uow;
    public ReactivarProveedorHandler(IUnitOfWork uow) => _uow = uow;

    public async Task<Result<Unit>> Handle(ReactivarProveedorCommand req, CancellationToken ct)
    {
        var p = await _uow.Proveedores.GetByIdAsync(req.Id, ct);
        if (p == null) return Result<Unit>.Failure("Proveedor no encontrado");
        p.Activar();
        await _uow.SaveChangesAsync(ct);
        return Result<Unit>.Success(Unit.Value);
    }
}
