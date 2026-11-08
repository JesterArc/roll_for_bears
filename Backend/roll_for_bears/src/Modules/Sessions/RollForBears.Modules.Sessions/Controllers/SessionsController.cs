using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RollForBears.Modules.Sessions.Contracts.Api;
using RollForBears.Modules.Sessions.Contracts.DTOs;

using Microsoft.AspNetCore.Http;
using Npgsql;

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

    [HttpGet]
    public async Task<IActionResult> GetSessionsAsync(CancellationToken cancellationToken, [FromQuery] int page=1, int pageSize=10)
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
        try
        {
            var sessions = await _sessionsApi.GetSessionsAsync(page, pageSize, cancellationToken);
            return Ok(sessions);
        }
        catch (OperationCanceledException)
        {
            return new StatusCodeResult(StatusCodes.Status499ClientClosedRequest);
        }
        catch (NpgsqlException)
        {
            return BadRequest("Invalid request");
        }
        catch (Exception)
        {
            return new StatusCodeResult(StatusCodes.Status500InternalServerError);
        }
    }
    
    [HttpGet("filtered")]
    public async Task<IActionResult> GetSessionsFilteredAsync([FromForm] SessionFilteredRequestDto dto, 
        CancellationToken cancellationToken, [FromQuery] int page=1, int pageSize=10)
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
        try
        {
            var sessions = await _sessionsApi.GetSessionsFilteredAsync(page, pageSize, 
                dto, cancellationToken);
            return Ok(sessions);
        }
        catch (OperationCanceledException)
        {
            return new StatusCodeResult(StatusCodes.Status499ClientClosedRequest);
        }
        catch (NpgsqlException)
        {
            return BadRequest("Invalid request");
        }
        catch (Exception)
        {
            return new StatusCodeResult(StatusCodes.Status500InternalServerError);
        }
    }
    
    [HttpPost("request/join")]
    public async Task<IActionResult> RequestJoinAsync([FromForm] SessionJoinRequestAddDto request,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var userUuid = GetUserUuid();
        if (userUuid == Guid.Empty)
        {
            return new StatusCodeResult(StatusCodes.Status401Unauthorized);
        }
        try
        {
            var updatedSessionUuid =
                await _sessionsApi.AddJoinRequestToSessionAsync(request, userUuid, cancellationToken);
            return CreatedAtRoute("GetSession", new { sessionId = updatedSessionUuid.ToString() }, updatedSessionUuid);
        }
        catch (OperationCanceledException)
        {
            return new StatusCodeResult(StatusCodes.Status499ClientClosedRequest);
        }
        catch (BadHttpRequestException exception)
        {
            return BadRequest(exception.Message);
        }
        catch (NpgsqlException)
        {
            return BadRequest("Invalid request");
        }
        catch (Exception)
        {
            return new StatusCodeResult(StatusCodes.Status500InternalServerError);
        }
    }

    [HttpPost("request/accept")]
    public async Task<IActionResult> AcceptJoinRequestAsync([FromForm] SessionPlayerAddDto request, 
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var userUuid = GetUserUuid();
        if (userUuid == Guid.Empty)
        {
            return new StatusCodeResult(StatusCodes.Status401Unauthorized);
        }
        try
        {
            var updatedSessionUuid = await _sessionsApi.AddPlayerToSessionAsync(request, userUuid, cancellationToken);
            return CreatedAtRoute("GetSession", new { sessionId = updatedSessionUuid.ToString() }, updatedSessionUuid);
        } catch (OperationCanceledException)
        {
            return new StatusCodeResult(StatusCodes.Status499ClientClosedRequest);
        }
        catch (BadHttpRequestException exception)
        {
            return BadRequest(exception.Message);
        }
        catch (NpgsqlException)
        {
            return BadRequest("Invalid request");
        }
        catch (Exception)
        {
            return new StatusCodeResult(StatusCodes.Status500InternalServerError);
        }
    }
    

    [HttpGet("{sessionId}", Name="GetSession")]
    public async Task<IActionResult> GetSpecificSessionAsync([FromRoute] string sessionId, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        try
        {
            var session = await _sessionsApi.GetSpecificSessionInfoAsync(sessionId, cancellationToken);
            if (session == null)
            {
                return NotFound("Session not found");
            }
            return Ok(session);
        }
        catch (OperationCanceledException)
        {
            return new StatusCodeResult(StatusCodes.Status499ClientClosedRequest);
        }
        catch (NpgsqlException)
        {
            return BadRequest("Invalid request");
        }
        catch (Exception)
        {
            return new StatusCodeResult(StatusCodes.Status500InternalServerError);
        }
    }

    [HttpPost]
    public async Task<IActionResult> CreateSessionAsync([FromForm] SessionCreateRequestDto request, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var ownerUuid = GetUserUuid();
        try
        {
            var newSessionUuid = await _sessionsApi.CreateSessionAsync(request, ownerUuid, cancellationToken);
            return CreatedAtRoute("GetSession", new { sessionId = newSessionUuid.ToString()}, newSessionUuid);
        }
        catch (OperationCanceledException)
        {
            return new StatusCodeResult(StatusCodes.Status499ClientClosedRequest);
        }
        catch (NpgsqlException)
        {
            return BadRequest("Invalid request - session not created");
        }
        catch (Exception)
        {
            return new StatusCodeResult(StatusCodes.Status500InternalServerError);
        }
    }

    private Guid GetUserUuid()
    {
        return Guid.Parse(User.FindFirstValue(JwtRegisteredClaimNames.Sub) ?? Guid.Empty.ToString());
    }
}