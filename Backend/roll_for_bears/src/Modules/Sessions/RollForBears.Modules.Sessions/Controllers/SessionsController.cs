using System.IdentityModel.Tokens.Jwt;
using System.Security.Authentication;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using RollForBears.Modules.Sessions.Contracts.Api;
using RollForBears.Modules.Sessions.Contracts.DTOs;

using Microsoft.AspNetCore.Http;
using Npgsql;
using RollForBears.Modules.Shared_Kernel.Exceptions;
using UnauthorizedAccessException = RollForBears.Modules.Shared_Kernel.Exceptions.UnauthorizedAccessException;

namespace RollForBears.Modules.Sessions.Controllers;

[ApiController]
[Route("api/[Controller]")]
public sealed class SessionsController : ControllerBase
{
    private ISessionsApi _sessionsApi;

    public SessionsController(ISessionsApi sessionsApi)
    {
        _sessionsApi = sessionsApi;
    }

    [HttpGet(Name="GetSessions")]
    public async Task<IActionResult> GetSessionsAsync(CancellationToken cancellationToken, int page=1, int pageSize=10)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (page <= 0)
        {
            return BadRequest("Page number cannot be less than or equal to zero");
        }
        if (pageSize <= 0)
        {
            return BadRequest("Page size cannot be less than or equal to zero");
        }
        var sessions = await _sessionsApi.GetSessionsAsync(page, pageSize, cancellationToken);
        return Ok(sessions);
    }
    
    [HttpGet("{sessionId}", Name="GetSession")]
    public async Task<IActionResult> GetSpecificSessionAsync([FromRoute] string sessionId, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (!Guid.TryParse(sessionId, out var sessionUuid)) return BadRequest("Invalid session identifier");
        var session = await _sessionsApi.GetSpecificSessionInfoAsync(sessionUuid, cancellationToken);
        return Ok(session);
    }
    
    [HttpPost("filtered")]
    public async Task<IActionResult> GetSessionsFilteredAsync([FromForm] SessionFilteredRequestDto filters, 
        CancellationToken cancellationToken, [FromQuery] int page=1, [FromQuery] int pageSize=10)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (page <= 0)
        {
            return BadRequest("Page number cannot be less than or equal to zero");
        }
        if (pageSize <= 0)
        {
            return BadRequest("Page size cannot be less than or equal to zero");
        }
        var sessions = await _sessionsApi.GetSessionsFilteredAsync(page, pageSize, 
                filters, cancellationToken);
        return Ok(sessions);
    }
    
    [HttpPost]
    public async Task<IActionResult> CreateSessionAsync([FromForm] SessionCreateRequestDto request, 
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var userUuid = GetUuidIfLoggedIn();
        var newSessionUuid = await _sessionsApi.CreateSessionAsync(request, userUuid, cancellationToken);
        return CreatedAtRoute("GetSession", new { sessionId = newSessionUuid.ToString()}, newSessionUuid);
    }
    
    [HttpPost("{sessionId}/join")]
    public async Task<IActionResult> RequestJoinAsync([FromRoute] string sessionId, [FromForm] SessionJoinRequestAddDto request,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var userUuid = GetUuidIfLoggedIn();
        if (!Guid.TryParse(sessionId, out var sessionUuid)) return BadRequest("Invalid session identifier");
        var updatedSessionUuid =
                await _sessionsApi.AddJoinRequestToSessionAsync(sessionUuid, request, userUuid, cancellationToken);
        return CreatedAtRoute("GetSession", new { sessionId = updatedSessionUuid.ToString() }, updatedSessionUuid);
    }
    
    [HttpPost("{sessionId}/accept")]
    public async Task<IActionResult> AcceptJoinRequestAsync([FromRoute] string sessionId, [FromForm] SessionPlayerAddDto request, 
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var userUuid = GetUuidIfLoggedIn();
        if (!Guid.TryParse(sessionId, out var sessionUuid)) return BadRequest("Invalid session identifier");
        var updatedSessionUuid = await _sessionsApi.AddPlayerToSessionAsync(sessionUuid, request, userUuid, cancellationToken);
        return CreatedAtRoute("GetSession", new { sessionId = updatedSessionUuid.ToString() }, updatedSessionUuid);
    }
    
    [HttpPut("{sessionId}/role")]
    public async Task<IActionResult> ChangePlayerRoleAsync([FromRoute] string sessionId, [FromForm] SessionPlayerAddDto request,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var userUuid = GetUuidIfLoggedIn();
        if (!Guid.TryParse(sessionId, out var sessionUuid)) return BadRequest("Invalid session identifier");
        var updatedSessionUuid = await _sessionsApi.ChangePlayerRoleAsync(sessionUuid, request, userUuid, cancellationToken);
        var uri = new Uri(Url.Link("GetSession", new { sessionId = updatedSessionUuid.ToString() }) ?? string.Empty);
        Response.Headers.Append("Location", uri.AbsoluteUri);
        return Ok();
    }

    [HttpPut("{sessionId}/info")]
    public async Task<IActionResult> UpdateSessionAsync([FromRoute] string sessionId, [FromForm] SessionUpdateRequestDto request,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var userUuid = GetUuidIfLoggedIn();
        if (!Guid.TryParse(sessionId, out var sessionUuid)) return BadRequest("Invalid session identifier");
        var updatedSessionUuid = await _sessionsApi.UpdateSessionInfoAsync(sessionUuid, request, userUuid, cancellationToken);
        var uri = new Uri(Url.Link("GetSession", new { sessionId = updatedSessionUuid.ToString() }) ?? string.Empty);
        Response.Headers.Append("Location", uri.AbsoluteUri);
        return Ok();
    }

    [HttpDelete("{sessionId}/request/{playerId}")]
    public async Task<IActionResult> RemoveJoinRequestAsync([FromRoute] string sessionId, [FromRoute] string playerId,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var userUuid = GetUuidIfLoggedIn();
        if (!Guid.TryParse(sessionId, out var sessionUuid)) return BadRequest("Invalid session identifier");
        if (!Guid.TryParse(playerId, out var playerUuid)) return BadRequest("Invalid player identifier");
        var request = new SessionJoinRequestRemoveDto
        {
            SessionId = sessionUuid,
            UserId = playerUuid
        };
        var updatedSessionUuid = await _sessionsApi.RemoveJoinRequestFromSessionAsync(request, userUuid, cancellationToken);
        var uri = new Uri(Url.Link("GetSession", new { sessionId = updatedSessionUuid.ToString() }) ?? string.Empty);
        Response.Headers.Append("Location", uri.AbsoluteUri);
        return Ok();
    }

    [HttpDelete("{sessionId}/player/{playerId}")]
    public async Task<IActionResult> RemovePlayerAsync([FromRoute] string sessionId, [FromRoute] string playerId,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var userUuid = GetUuidIfLoggedIn();
        if (!Guid.TryParse(sessionId, out var sessionUuid)) return BadRequest("Invalid session identifier");
        if (!Guid.TryParse(playerId, out var playerUuid)) return BadRequest("Invalid player identifier");
        var request = new SessionPlayerRemoveDto
        {
            SessionId = sessionUuid,
            AccountId = playerUuid
        };
        var updatedSessionUuid = await _sessionsApi.RemovePlayerAsync(request, userUuid, cancellationToken);
        var uri = new Uri(Url.Link("GetSession", new { sessionId = request.SessionId.ToString() }) ?? string.Empty);
        Response.Headers.Append("Location", uri.AbsoluteUri);
        if (updatedSessionUuid == Guid.Empty)
        {
            return NoContent();
        }
        return Ok();
    }

    [HttpDelete("{sessionId}")]
    public async Task<IActionResult> DeleteSessionAsync([FromRoute] string sessionId,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var userUuid =  GetUuidIfLoggedIn();
        if (!Guid.TryParse(sessionId, out var sessionUuid)) return BadRequest("Invalid session identifier");
        await _sessionsApi.DeleteSessionAsync(sessionUuid, userUuid, cancellationToken);
        var uri = new Uri(Url.Link("GetSessions", new { }) ?? string.Empty);
        Response.Headers.Append("Location", uri.AbsoluteUri);
        return NoContent();
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