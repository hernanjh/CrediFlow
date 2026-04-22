using CrediFlow.Application.Common;
using CrediFlow.Application.DTOs;
using CrediFlow.Domain.Entities;
using CrediFlow.Domain.Interfaces;
using MediatR;

namespace CrediFlow.Application.Features.Seguridad;

// ─── QUERIES ─────────────────────────────────────────────────────────────
public record GetPerfilesQuery() : IRequest<Result<IEnumerable<PerfilDto>>>;

public class GetPerfilesHandler : IRequestHandler<GetPerfilesQuery, Result<IEnumerable<PerfilDto>>>
{
    private readonly IUnitOfWork _uow;
    public GetPerfilesHandler(IUnitOfWork uow) => _uow = uow;

    public async Task<Result<IEnumerable<PerfilDto>>> Handle(GetPerfilesQuery request, CancellationToken ct)
    {
        var perfiles = await _uow.Perfiles.GetAllAsync(ct);
        var dtos = perfiles.Select(p => new PerfilDto(
            p.Id, p.Nombre, p.Descripcion, true,
            p.Permisos.Select(pp => new PermisoDto(pp.Permiso!.Id, pp.Permiso.Nombre, pp.Permiso.Descripcion, pp.Permiso.Seccion, true)).ToList()
        ));
        return Result<IEnumerable<PerfilDto>>.Success(dtos);
    }
}

public record GetPermisosQuery() : IRequest<Result<IEnumerable<PermisoDto>>>;

public class GetPermisosHandler : IRequestHandler<GetPermisosQuery, Result<IEnumerable<PermisoDto>>>
{
    private readonly IUnitOfWork _uow;
    public GetPermisosHandler(IUnitOfWork uow) => _uow = uow;

    public async Task<Result<IEnumerable<PermisoDto>>> Handle(GetPermisosQuery request, CancellationToken ct)
    {
        var permisos = await _uow.Perfiles.GetAllPermisosAsync(ct);
        var dtos = permisos.Select(p => new PermisoDto(p.Id, p.Nombre, p.Descripcion, p.Seccion, true));
        return Result<IEnumerable<PermisoDto>>.Success(dtos);
    }
}

public record GetUsuariosQuery() : IRequest<Result<IEnumerable<UsuarioDto>>>;
public record ReactivarUsuarioCommand(Guid Id) : IRequest<Result>;
public record ReactivarPerfilCommand(Guid Id) : IRequest<Result>;

public class GetUsuariosHandler : IRequestHandler<GetUsuariosQuery, Result<IEnumerable<UsuarioDto>>>
{
    private readonly IUnitOfWork _uow;
    public GetUsuariosHandler(IUnitOfWork uow) => _uow = uow;

    public async Task<Result<IEnumerable<UsuarioDto>>> Handle(GetUsuariosQuery request, CancellationToken ct)
    {
        var users = await _uow.Usuarios.GetAllAsync(ct);
        var dtos = users.Select(u => new UsuarioDto(
            u.Id, u.NombreCompleto, u.Email, u.Rol, u.Activo, u.FotoPerfil, u.PorcentajeComision, u.PerfilId, u.Perfil?.Nombre
        ));
        return Result<IEnumerable<UsuarioDto>>.Success(dtos);
    }
}

// ─── COMMANDS ────────────────────────────────────────────────────────────
public record CrearPerfilCommand(string Nombre, string? Descripcion, List<Guid> PermisosIds) : IRequest<Result<Guid>>;

public class CrearPerfilHandler : IRequestHandler<CrearPerfilCommand, Result<Guid>>
{
    private readonly IUnitOfWork _uow;
    public CrearPerfilHandler(IUnitOfWork uow) => _uow = uow;

    public async Task<Result<Guid>> Handle(CrearPerfilCommand req, CancellationToken ct)
    {
        var p = new Perfil { Nombre = req.Nombre, Descripcion = req.Descripcion };
        foreach (var pid in req.PermisosIds) p.AgregarPermiso(pid);
        
        await _uow.Perfiles.AddAsync(p, ct);
        await _uow.SaveChangesAsync(ct);
        return Result<Guid>.Success(p.Id);
    }
}

public record ActualizarUsuarioCommand(Guid Id, string NombreCompleto, Guid? PerfilId, string? FotoPerfil, decimal Comision) : IRequest<Result>;

public class ActualizarUsuarioHandler : IRequestHandler<ActualizarUsuarioCommand, Result>
{
    private readonly IUnitOfWork _uow;
    public ActualizarUsuarioHandler(IUnitOfWork uow) => _uow = uow;

    public async Task<Result> Handle(ActualizarUsuarioCommand req, CancellationToken ct)
    {
        var u = await _uow.Usuarios.GetByIdAsync(req.Id, ct);
        if (u == null) return Result.Failure("Usuario no encontrado");

        u.Actualizar(req.NombreCompleto, req.FotoPerfil, req.Comision);
        // u.AsignarPerfil(req.PerfilId); // Necesitamos este método en Usuario.cs o hacerlo directo
        // u.PerfilId = req.PerfilId; // PerfilId es private set, necesito un método

        await _uow.SaveChangesAsync(ct);
        return Result.Success();
    }
}

public class ReactivarUsuarioHandler : IRequestHandler<ReactivarUsuarioCommand, Result>
{
    private readonly IUnitOfWork _uow;
    public ReactivarUsuarioHandler(IUnitOfWork uow) => _uow = uow;

    public async Task<Result> Handle(ReactivarUsuarioCommand req, CancellationToken ct)
    {
        var u = await _uow.Usuarios.GetByIdAsync(req.Id, ct);
        if (u == null) return Result.Failure("Usuario no encontrado");
        u.Activar();
        await _uow.SaveChangesAsync(ct);
        return Result.Success();
    }
}

public class ReactivarPerfilHandler : IRequestHandler<ReactivarPerfilCommand, Result>
{
    private readonly IUnitOfWork _uow;
    public ReactivarPerfilHandler(IUnitOfWork uow) => _uow = uow;

    public async Task<Result> Handle(ReactivarPerfilCommand req, CancellationToken ct)
    {
        var p = await _uow.Perfiles.GetByIdAsync(req.Id, ct);
        if (p == null) return Result.Failure("Perfil no encontrado");
        p.Activar();
        await _uow.SaveChangesAsync(ct);
        return Result.Success();
    }
}
