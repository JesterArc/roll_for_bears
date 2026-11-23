using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
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
    public async Task<ActionResult> RegisterAsync([FromBody] RegisterRequestDto request, 
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        await _usersApi.RegisterAsync(request, cancellationToken);
        return Ok();
    }

    [HttpPost("login")]
    public async Task<ActionResult<LoginResultDto>> LoginAsync([FromBody] LoginRequestDto request, 
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
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
        cancellationToken.ThrowIfCancellationRequested();
        var token = Request.Cookies["refreshToken"];
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
        cancellationToken.ThrowIfCancellationRequested();
        var token = Request.Cookies["refreshToken"];
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

    [HttpGet("friend")]
    public async Task<IActionResult> GetFriendsAsync(CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var userUuid = GetUuidIfLoggedIn();
        
        var friends = await _usersApi.GetFriendsAsync(userUuid, cancellationToken);
        return Ok(friends);
    }
    
    [HttpGet("friend/requests")]
    public async Task<IActionResult> GetFriendRequests(CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var userUuid = GetUuidIfLoggedIn();
        
        var requests = await _usersApi.GetFriendRequestsAsync(userUuid, cancellationToken);
        return Ok(new
        {
            Incoming = requests.IncomingFriendRequests,
            Outgoing = requests.OutgoingFriendRequests
        });
    }
    
    
    
    [HttpPost("friend/send")]
    public async Task<IActionResult> SendFriendRequestAsync([FromBody] FriendRequestDto request, 
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var userUuid = GetUuidIfLoggedIn();
        
        await _usersApi.SendFriendInviteAsync(userUuid, request.FriendUuid, cancellationToken);
        return Ok("Friend request sent successfully");
    }

    [HttpPut("friend/accept")]
    public async Task<IActionResult> AcceptFriendRequestAsync([FromBody] FriendRequestDto request, 
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var userUuid = GetUuidIfLoggedIn();
        
        await _usersApi.AcceptFriendRequestAsync(userUuid, request.FriendUuid, cancellationToken);
        return Ok("You are friends now");
    }

    [HttpDelete("friend/reject")]
    public async Task<IActionResult> RejectFriendRequestAsync([FromBody] FriendRequestDto request, 
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var userUuid = GetUuidIfLoggedIn();
        
        await _usersApi.RejectFriendAsync(userUuid, request.FriendUuid, cancellationToken);
        return Ok("Friend request rejected");
    }
    
    [HttpDelete("friend/remove")]
    public async Task<IActionResult> RemoveFriendAsync([FromBody] FriendRequestDto request, 
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var userUuid = GetUuidIfLoggedIn();
        
        await _usersApi.RejectFriendAsync(userUuid, request.FriendUuid, cancellationToken);
        return Ok("Friend removed");
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
    
    private Guid GetUuidIfLoggedIn()
    {
        var uuid = Guid.Parse(User.FindFirstValue(JwtRegisteredClaimNames.Sub) ?? Guid.Empty.ToString());
        if (uuid == Guid.Empty)
        {
            throw new UnauthorizedAccessException("You are not logged in");
        }
        return uuid;
    }
}