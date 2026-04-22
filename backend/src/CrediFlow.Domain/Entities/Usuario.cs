using CrediFlow.Domain.Common;
using CrediFlow.Domain.Enums;

namespace CrediFlow.Domain.Entities;

public class Usuario : BaseEntity
{
    public string NombreCompleto { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public string PasswordHash { get; private set; } = string.Empty;
    public TipoUsuario Rol { get; private set; } // Rol de sistema (atenuación)
    public Guid? PerfilId { get; private set; }
    public Perfil? Perfil { get; private set; }
    public bool Activo { get; private set; } = true;
    public string? FotoPerfil { get; private set; }
    public decimal PorcentajeComision { get; private set; } = 0;
    public decimal AdelantoRetirado { get; private set; } = 0;

    private readonly List<UsuarioZona> _zonas = new();
    public IReadOnlyCollection<UsuarioZona> Zonas => _zonas.AsReadOnly();

    private readonly List<RefreshToken> _refreshTokens = new();
    public IReadOnlyCollection<RefreshToken> RefreshTokens => _refreshTokens.AsReadOnly();

    protected Usuario() { }

    public static Usuario Crear(string nombreCompleto, string email, string passwordHash, TipoUsuario rol, decimal porcentajeComision = 0)
    {
        if (string.IsNullOrWhiteSpace(email)) throw new ArgumentException("El email es requerido.");
        return new Usuario
        {
            NombreCompleto = nombreCompleto,
            Email = email.ToLower().Trim(),
            PasswordHash = passwordHash,
            Rol = rol,
            PorcentajeComision = porcentajeComision
        };
    }

    public void Actualizar(string nombreCompleto, string? fotoPerfil, decimal porcentajeComision)
    {
        NombreCompleto = nombreCompleto;
        FotoPerfil = fotoPerfil;
        PorcentajeComision = porcentajeComision;
    }

    public void AsignarPerfil(Guid? perfilId) => PerfilId = perfilId;


    public void CambiarPassword(string nuevoHash) => PasswordHash = nuevoHash;
    public void Desactivar() => Activo = false;
    public void Activar() => Activo = true;

    public bool EsSuperAdmin => Rol == TipoUsuario.SUPER_ADMIN;
    public bool EsVendedor => Rol == TipoUsuario.VENDEDOR;
}

public class RefreshToken
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid UsuarioId { get; set; }
    public string Token { get; set; } = string.Empty;
    public DateTime Expiracion { get; set; }
    public bool Revocado { get; set; }
    public string? DireccionIp { get; set; }
    public Usuario? Usuario { get; set; }

    public bool EsValido => !Revocado && Expiracion > DateTime.UtcNow;
}
