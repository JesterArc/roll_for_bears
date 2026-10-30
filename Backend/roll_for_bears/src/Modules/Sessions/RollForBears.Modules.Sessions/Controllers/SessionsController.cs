using System.Net;
using Microsoft.AspNetCore.Mvc;
using RollForBears.Modules.Sessions.Contracts.Api;
using RollForBears.Modules.Sessions.Contracts.DTOs;
using System.Threading;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
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
    public async Task<IActionResult> GetSessionsAsync(CancellationToken cancellationToken, [FromQuery] int page=0, int pageSize=10)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (page < 0)
        {
            return BadRequest("Page number cannot be less than zero");
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
        CancellationToken cancellationToken, [FromQuery] int page=0, int pageSize=10)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (page < 0)
        {
            return BadRequest("Page number cannot be less than zero");
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
        try
        {
            var newSessionUuid = await _sessionsApi.CreateSessionAsync(request, cancellationToken);
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
}