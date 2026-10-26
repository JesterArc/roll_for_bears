using System.Net;
using Microsoft.AspNetCore.Mvc;
using RollForBears.Modules.Sessions.Contracts.Api;
using RollForBears.Modules.Sessions.Contracts.DTOs;
using System.Threading;
using Microsoft.AspNetCore.Http;

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
    public async Task<IActionResult> GetSessionsAsync(CancellationToken cancellationToken, int page=0, int pageSize=10)
    {
        var source = new CancellationTokenSource();
        try
        {
            source.CancelAfter(100000);
            var sessions = await _sessionsApi.GetSessionsAsync(page, pageSize, cancellationToken);
            return Ok(sessions);
        }
        catch (OperationCanceledException)
        {
            return new StatusCodeResult((int)HttpStatusCode.RequestTimeout);
        }
        finally
        {
            source.Dispose();
        }
    }

    [HttpGet("{sessionId}", Name="GetSession")]
    public async Task<IActionResult> GetSpecificSessionAsync([FromRoute] string sessionId, CancellationToken cancellationToken)
    {
        try
        {
            var session = await _sessionsApi.GetSpecificSessionInfoAsync(sessionId, cancellationToken);
            return Ok(session);
        }
        catch (OperationCanceledException)
        {
            return new StatusCodeResult(StatusCodes.Status499ClientClosedRequest);
        }
        catch (Exception)
        {
            return BadRequest("Failed to create session");
        }
    }

    [HttpPost]
    public async Task<IActionResult> CreateSessionAsync([FromForm] SessionCreateRequestDto request, CancellationToken cancellationToken)
    {
        try
        {
            var newSessionUuid = await _sessionsApi.CreateSessionAsync(request, cancellationToken);
            return CreatedAtRoute("GetSession", new { sessionId = newSessionUuid.ToString()}, newSessionUuid);
        }
        catch (OperationCanceledException)
        {
            return new StatusCodeResult(StatusCodes.Status499ClientClosedRequest);
        }
        catch (Exception)
        {
            return BadRequest("Failed to create session");
        }
    }
}