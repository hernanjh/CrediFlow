using CrediFlow.Domain.Common;

namespace CrediFlow.Domain.Entities;

public class Permiso : BaseEntity
{
    public string Nombre { get; set; } = string.Empty; // e.g. "CLIENTES_VER"
    public string Descripcion { get; set; } = string.Empty;
    public string Seccion { get; set; } = string.Empty; // e.g. "PRODUCTOS"
    public bool Activo { get; private set; } = true;

    public void Activar() => Activo = true;
    public void Desactivar() => Activo = false;
}

public class Perfil : BaseEntity
{
    public string Nombre { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public bool Activo { get; private set; } = true;

    public void Activar() => Activo = true;
    public void Desactivar() => Activo = false;

    private readonly List<PerfilPermiso> _permisos = new();
    public IReadOnlyCollection<PerfilPermiso> Permisos => _permisos.AsReadOnly();

    public void AgregarPermiso(Guid permisoId)
    {
        if (!_permisos.Any(p => p.PermisoId == permisoId))
        {
            _permisos.Add(new PerfilPermiso { PerfilId = Id, PermisoId = permisoId });
        }
    }
}

public class PerfilPermiso
{
    public Guid PerfilId { get; set; }
    public Perfil? Perfil { get; set; }
    public Guid PermisoId { get; set; }
    public Permiso? Permiso { get; set; }
}
