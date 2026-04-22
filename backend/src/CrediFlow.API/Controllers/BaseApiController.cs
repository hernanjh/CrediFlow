using CrediFlow.Application.Common;
using CrediFlow.Application.DTOs;
using CrediFlow.Domain.Entities;
using CrediFlow.Domain.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CrediFlow.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public abstract class BaseApiController : ControllerBase
{
    protected IActionResult HandleResult<T>(Result<T> result)
    {
        if (result.IsSuccess)
            return Ok(result.Value);
        if (result.ValidationErrors.Any())
            return BadRequest(new { Errors = result.ValidationErrors });
        if (result.Error?.Contains("no encontrado") == true)
            return NotFound(new { Error = result.Error });
        if (result.Error?.Contains("denegado") == true || result.Error?.Contains("bloqueado") == true)
            return Forbid();
        return BadRequest(new { Error = result.Error });
    }

    protected IActionResult HandleResult(Result result)
    {
        if (result.IsSuccess) return NoContent();
        if (result.ValidationErrors.Any())
            return BadRequest(new { Errors = result.ValidationErrors });
        return BadRequest(new { Error = result.Error });
    }

    protected Guid? CurrentUserId
    {
        get
        {
            var sub = User.FindFirstValue(ClaimTypes.NameIdentifier) ??
                      User.FindFirstValue(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub);
            return Guid.TryParse(sub, out var id) ? id : null;
        }
    }
}
