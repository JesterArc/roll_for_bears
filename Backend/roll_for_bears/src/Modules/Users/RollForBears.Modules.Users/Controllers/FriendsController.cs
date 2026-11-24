using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using RollForBears.Modules.Users.Contracts.Api;
using RollForBears.Modules.Users.Contracts.DTOs;

namespace RollForBears.Modules.Users.Controllers;
[ApiController]
[Route("api/[controller]")]
public class FriendsController : ControllerBase
{
    private readonly IFriendsApi _friendsApi;

    public FriendsController(IFriendsApi friendsApi)
    {
        _friendsApi = friendsApi;
    }
    [HttpGet]
    public async Task<IActionResult> GetFriendsAsync(CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var userUuid = GetUuidIfLoggedIn();
        
        var friends = await _friendsApi.GetFriendsAsync(userUuid, cancellationToken);
        return Ok(new
        {
            FriendList = friends
        });
    }
    
    [HttpGet("requests")]
    public async Task<IActionResult> GetFriendRequests(CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var userUuid = GetUuidIfLoggedIn();
        
        var requests = await _friendsApi.GetFriendRequestsAsync(userUuid, cancellationToken);
        return Ok(new
        {
            Incoming = requests.IncomingFriendRequests,
            Outgoing = requests.OutgoingFriendRequests
        });
    }
    
    [HttpPost("send")]
    public async Task<IActionResult> SendFriendRequestAsync([FromBody] FriendRequestDto request, 
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var userUuid = GetUuidIfLoggedIn();
        
        await _friendsApi.SendFriendInviteAsync(userUuid, request.FriendUuid, cancellationToken);
        return Ok("Friend request sent successfully");
    }

    [HttpPut("accept")]
    public async Task<IActionResult> AcceptFriendRequestAsync([FromBody] FriendRequestDto request, 
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var userUuid = GetUuidIfLoggedIn();
        
        await _friendsApi.AcceptFriendRequestAsync(userUuid, request.FriendUuid, cancellationToken);
        return Ok("You are friends now");
    }

    [HttpDelete("reject")]
    public async Task<IActionResult> RejectFriendRequestAsync([FromBody] FriendRequestDto request, 
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var userUuid = GetUuidIfLoggedIn();
        
        await _friendsApi.RejectFriendAsync(userUuid, request.FriendUuid, cancellationToken);
        return Ok("Friend request rejected");
    }
    
    [HttpDelete("remove")]
    public async Task<IActionResult> RemoveFriendAsync([FromBody] FriendRequestDto request, 
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var userUuid = GetUuidIfLoggedIn();
        
        await _friendsApi.RejectFriendAsync(userUuid, request.FriendUuid, cancellationToken);
        return Ok("Friend removed");
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