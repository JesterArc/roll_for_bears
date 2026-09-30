using Microsoft.AspNetCore.Mvc;
using RollForBears.Modules.Sessions.Contracts.Api;
using RollForBears.Modules.Sessions.Contracts.DTOs;

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
        var sessions = await _sessionsApi.GetSessionsAsync(page, pageSize);
        return Ok(sessions);
    }

    [HttpPost]
    public async Task<IActionResult> CreateSessionAsync([FromBody] SessionCreateRequestDto request)
    {
        return Ok("Not implemented yet");
    }
}