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
    public async Task<ActionResult> RegisterAsync(RegisterRequestDto request)
    {
        await _usersApi.RegisterAsync(request);
        return Ok();
    }

    [HttpPost("login")]
    public async Task<ActionResult<LoginResultDto>> LoginAsync(LoginRequestDto request)
    {
        var result = await _usersApi.LoginAsync(request);
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
    public async Task<ActionResult<LoginResultDto>> RefreshTokenAsync(string refreshToken)
    {
        string? token = Request.Cookies["refreshToken"];
        if (string.IsNullOrEmpty(token))
            return Unauthorized();
        
        var result = await _usersApi.RefreshTokenAsync(token);
        if (result == null)
        {
            return Unauthorized();
        }
        
        AddRefreshTokenCookie(result.RefreshToken);
        
        return Ok(result);
    }

    [HttpPost("logout")]
    public async Task<IActionResult> LogoutAsync(string refreshToken)
    {
        string? token = Request.Cookies["refreshToken"];
        if (!string.IsNullOrEmpty(token))
            await _usersApi.LogoutAsync(token);
        
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