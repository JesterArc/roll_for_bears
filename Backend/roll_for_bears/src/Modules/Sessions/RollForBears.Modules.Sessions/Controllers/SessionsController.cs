using System.Net;
using Microsoft.AspNetCore.Mvc;
using RollForBears.Modules.Sessions.Contracts.Api;
using RollForBears.Modules.Sessions.Contracts.DTOs;
using System.Threading;

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
    public async Task<IActionResult> GetSessionsAsync(int page=0, int pageSize=10)
    {
        var source = new CancellationTokenSource();
        try
        {
            source.CancelAfter(100000);
            var sessions = await _sessionsApi.GetSessionsAsync(page, pageSize);
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

    [HttpGet("{sessionId}")]
    public async Task<IActionResult> GetSpecificSessionAsync([FromRoute] string sessionId)
    {
        var source = new CancellationTokenSource();
        try
        {
            source.CancelAfter(100000);
            var session = await _sessionsApi.GetSpecificSessionInfoAsync(sessionId);
            return Ok(session);
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

    [HttpPost("/create")]
    public async Task<IActionResult> CreateSessionAsync([FromBody] SessionCreateRequestDto request)
    {
        return Ok("Not implemented yet");
    }
}