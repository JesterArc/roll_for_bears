using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RollForBears.Modules.Users.Contracts.Api;
using RollForBears.Modules.Users.Contracts.DTOs;

namespace RollForBears.Modules.Users.Controllers;

[ApiController]
[Route("api/users")]
public sealed class UsersController : ControllerBase
{
    private readonly IUsersApi _usersApi;

    public UsersController(IUsersApi usersApi)
    {
        _usersApi = usersApi;
    }

    [HttpPost("register")]
    public async Task<ActionResult> RegisterAsync(RegisterRequestDto request, CancellationToken cancellationToken)
    {
        await _usersApi.RegisterAsync(request, cancellationToken);
        return Ok();
    }

    [HttpPost("login")]
    public async Task<ActionResult<LoginResultDto>> LoginAsync(LoginRequestDto request, CancellationToken cancellationToken)
    {
        var result = await _usersApi.LoginAsync(request, cancellationToken);
        if (result == null)
        {
            return Unauthorized();
        }
        
        AddRefreshTokenCookie(result.RefreshToken);
        
        return Ok(new
        {
            result.Uuid,
            result.Username,
            result.AccessToken,
        });
    }

    [HttpPost("refresh")]
    public async Task<ActionResult<LoginResultDto>> RefreshTokenAsync(CancellationToken cancellationToken)
    {
        string? token = Request.Cookies["refreshToken"];
        if (string.IsNullOrEmpty(token))
            return Unauthorized();
        
        var result = await _usersApi.RefreshTokenAsync(token, cancellationToken);
        if (result == null)
        {
            return Unauthorized();
        }
        
        AddRefreshTokenCookie(result.RefreshToken);
        
        return Ok(result);
    }

    [HttpPost("logout")]
    public async Task<IActionResult> LogoutAsync(CancellationToken cancellationToken)
    {
        string? token = Request.Cookies["refreshToken"];
        if (!string.IsNullOrEmpty(token))
            await _usersApi.LogoutAsync(token, cancellationToken);
        
        Response.Cookies.Delete("refreshToken",
                new CookieOptions
                {
                    HttpOnly = true,
                    Secure = false,
                    SameSite = SameSiteMode.Lax,
                    Path = "/"
                }
            );
        
        return NoContent();
    }
    
    
    private void AddRefreshTokenCookie(string token)
    {
        Response.Cookies.Append("refreshToken", token,
            new CookieOptions
            {
                HttpOnly = true,
                Secure = false,
                SameSite = SameSiteMode.Lax,
                Expires = DateTimeOffset.UtcNow.AddHours(48),
                Path = "/"
            });
    }
}