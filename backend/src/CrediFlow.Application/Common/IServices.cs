namespace CrediFlow.Application.Common;

/// <summary>Servicio para obtener el usuario autenticado actual del contexto HTTP</summary>
public interface ICurrentUserService
{
    Guid? UserId { get; }
    string? Email { get; }
    string? Role { get; }
    bool IsSuperAdmin { get; }
    bool IsAdmin { get; }
    bool IsVendedor { get; }
    string? IpAddress { get; }
}

/// <summary>Servicio para generación y validación de JWT</summary>
public interface ITokenService
{
    string GenerateAccessToken(Guid userId, string email, string role);
    string GenerateRefreshToken();
    Guid? ValidateTokenAndGetUserId(string token);
}

/// <summary>Servicio de hash de passwords</summary>
public interface IPasswordService
{
    string Hash(string password);
    bool Verify(string password, string hash);
}

/// <summary>Motor de cuotas: responsable de generar la tabla de cuotas de un crédito</summary>
public interface IMotorCuotasService
{
    Task<List<(int Numero, DateTime Fecha, decimal Monto)>> GenerarCuotasAsync(
        decimal capital, decimal tasaInteres, int numeroCuotas,
        CrediFlow.Domain.Enums.FrecuenciaCuota frecuencia,
        DateTime fechaPrimerVencimiento,
        CancellationToken ct = default);
}

/// <summary>Servicio de almacenamiento de archivos (fotos)</summary>
public interface IStorageService
{
    Task<string> UploadAsync(Stream stream, string fileName, string contentType, CancellationToken ct = default);
    Task DeleteAsync(string fileUrl, CancellationToken ct = default);
}

/// <summary>Servicio para disparar notificaciones/alertas</summary>
public interface INotificacionService
{
    Task NotificarSuperAdminAsync(string titulo, string mensaje, CancellationToken ct = default);
    Task NotificarUsuarioAsync(Guid usuarioId, string titulo, string mensaje, CancellationToken ct = default);
}
