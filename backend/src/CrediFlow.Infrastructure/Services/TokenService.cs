using CrediFlow.Application.Common;
using System.Security.Cryptography;
using Microsoft.Extensions.Configuration;

namespace CrediFlow.Infrastructure.Services;

public class TokenService : ITokenService
{
    private readonly IConfiguration _config;

    public TokenService(IConfiguration config) => _config = config;

    public string GenerateAccessToken(Guid userId, string email, string role)
    {
        // Delegamos la generación al servicio en API layer
        // Esta implementación usa el mismo código pero con las dependencias correctas
        var key = System.Text.Encoding.UTF8.GetBytes(_config["Jwt:Key"]!);
        var header = Base64UrlEncode(System.Text.Json.JsonSerializer.SerializeToUtf8Bytes(
            new { alg = "HS256", typ = "JWT" }));
        
        var now = DateTimeOffset.UtcNow;
        var payload = Base64UrlEncode(System.Text.Json.JsonSerializer.SerializeToUtf8Bytes(
            new {
                sub = userId.ToString(),
                email,
                role,
                iss = _config["Jwt:Issuer"],
                aud = _config["Jwt:Audience"],
                iat = now.ToUnixTimeSeconds(),
                exp = now.AddMinutes(15).ToUnixTimeSeconds(),
                jti = Guid.NewGuid().ToString()
            }));

        var signingInput = $"{header}.{payload}";
        var signature = Base64UrlEncode(
            new System.Security.Cryptography.HMACSHA256(key)
                .ComputeHash(System.Text.Encoding.UTF8.GetBytes(signingInput)));

        return $"{header}.{payload}.{signature}";
    }

    public string GenerateRefreshToken()
    {
        var bytes = RandomNumberGenerator.GetBytes(64);
        return Convert.ToBase64String(bytes);
    }

    public Guid? ValidateTokenAndGetUserId(string token)
    {
        try
        {
            var parts = token.Split('.');
            if (parts.Length != 3) return null;

            var payloadJson = System.Text.Encoding.UTF8.GetString(Base64UrlDecodeBytes(parts[1]));
            var payload = System.Text.Json.JsonDocument.Parse(payloadJson);
            
            if (payload.RootElement.TryGetProperty("sub", out var sub))
                return Guid.TryParse(sub.GetString(), out var id) ? id : null;
            
            return null;
        }
        catch { return null; }
    }

    private static string Base64UrlEncode(byte[] input)
        => Convert.ToBase64String(input).TrimEnd('=').Replace('+', '-').Replace('/', '_');

    private static byte[] Base64UrlDecodeBytes(string input)
    {
        var s = input.Replace('-', '+').Replace('_', '/');
        s += (s.Length % 4) switch { 2 => "==", 3 => "=", _ => "" };
        return Convert.FromBase64String(s);
    }
}

public class PasswordService : IPasswordService
{
    public string Hash(string password) => BCrypt.Net.BCrypt.HashPassword(password, workFactor: 12);
    public bool Verify(string password, string hash) => BCrypt.Net.BCrypt.Verify(password, hash);
}
