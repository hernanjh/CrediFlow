using CrediFlow.Application.Common;
using CrediFlow.Application.DTOs;
using CrediFlow.Domain.Entities;
using CrediFlow.Domain.Enums;
using CrediFlow.Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CrediFlow.API.Controllers;

[Authorize]
public class AuthController : BaseApiController
{
    private readonly IUnitOfWork _uow;
    private readonly ITokenService _tokenService;
    private readonly IPasswordService _passwordService;

    public AuthController(IUnitOfWork uow, ITokenService tokenService, IPasswordService passwordService)
    {
        _uow = uow;
        _tokenService = tokenService;
        _passwordService = passwordService;
    }

    [AllowAnonymous]
    [HttpPost("login")]
    [ProducesResponseType(typeof(LoginResponseDto), 200)]
    public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken ct)
    {
        var usuario = await _uow.Usuarios.GetByEmailAsync(request.Email, ct);
        if (usuario is null || !_passwordService.Verify(request.Password, usuario.PasswordHash))
            return Unauthorized(new { Error = "Credenciales inválidas." });

        if (!usuario.Activo)
            return Unauthorized(new { Error = "Usuario desactivado." });

        var accessToken = _tokenService.GenerateAccessToken(usuario.Id, usuario.Email, usuario.Rol.ToString());
        var refreshToken = _tokenService.GenerateRefreshToken();

        var rt = new RefreshToken
        {
            Token = refreshToken,
            UsuarioId = usuario.Id,
            Expiracion = DateTime.UtcNow.AddDays(7),
            DireccionIp = HttpContext.Connection.RemoteIpAddress?.ToString()
        };
        await _uow.Usuarios.AddRefreshTokenAsync(rt, ct);
        await _uow.SaveChangesAsync(ct);

        var usuarioDto = new UsuarioDto(usuario.Id, usuario.NombreCompleto, usuario.Email,
            usuario.Rol, usuario.Activo, usuario.FotoPerfil, usuario.PorcentajeComision, usuario.PerfilId, usuario.Perfil?.Nombre);

        var response = new LoginResponseDto(accessToken, refreshToken,
            DateTime.UtcNow.AddMinutes(15), usuarioDto);

        return Ok(response);
    }

    [AllowAnonymous]
    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh([FromBody] RefreshRequest request, CancellationToken ct)
    {
        var rt = await _uow.Usuarios.GetRefreshTokenAsync(request.RefreshToken, ct);
        if (rt is null || !rt.EsValido)
            return Unauthorized(new { Error = "Refresh token inválido." });

        var usuario = rt.Usuario!;
        rt.Revocado = true;
        _uow.Usuarios.Update(usuario);

        var newAccess = _tokenService.GenerateAccessToken(usuario.Id, usuario.Email, usuario.Rol.ToString());
        var newRefresh = _tokenService.GenerateRefreshToken();

        var newRt = new RefreshToken
        {
            Token = newRefresh,
            UsuarioId = usuario.Id,
            Expiracion = DateTime.UtcNow.AddDays(7)
        };
        await _uow.Usuarios.AddRefreshTokenAsync(newRt, ct);
        await _uow.SaveChangesAsync(ct);

        return Ok(new { AccessToken = newAccess, RefreshToken = newRefresh });
    }

    [HttpGet("me")]
    public async Task<IActionResult> Me(CancellationToken ct)
    {
        if (CurrentUserId is null) return Unauthorized();
        var usuario = await _uow.Usuarios.GetByIdAsync(CurrentUserId.Value, ct);
        if (usuario is null) return NotFound();
        var dto = new UsuarioDto(usuario.Id, usuario.NombreCompleto, usuario.Email,
            usuario.Rol, usuario.Activo, usuario.FotoPerfil, usuario.PorcentajeComision, usuario.PerfilId, usuario.Perfil?.Nombre);
        return Ok(dto);
    }

    [HttpPost("register")]
    public async Task<IActionResult> RegisterUser([FromBody] RegisterRequest req, CancellationToken ct)
    {
        var hash = _passwordService.Hash(req.Password);
        var rol = Enum.Parse<TipoUsuario>(req.Rol);
        var u = Usuario.Crear(req.NombreCompleto, req.Email, hash, rol);
        if(req.PerfilId.HasValue) u.AsignarPerfil(req.PerfilId.Value);

        await _uow.Usuarios.AddAsync(u, ct);
        await _uow.SaveChangesAsync(ct);
        return Ok(new { u.Id });
    }
}

public record RegisterRequest(string NombreCompleto, string Email, string Password, string Rol, Guid? PerfilId);
public record LoginRequest(string Email, string Password);
public record RefreshRequest(string RefreshToken);
