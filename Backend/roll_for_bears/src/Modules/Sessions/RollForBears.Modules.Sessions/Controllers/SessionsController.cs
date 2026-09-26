using Microsoft.AspNetCore.Mvc;
using RollForBears.Modules.Sessions.Contracts.Api;

namespace RollForBears.Modules.Sessions.Controllers;

[ApiController]
[Route("api/sessions")]
public sealed class SessionsController : ControllerBase
{
    private ISessionsApi _sessionsApi;

    public SessionsController(ISessionsApi sessionsApi)
    {
        _sessionsApi = sessionsApi;
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        return Ok("Not implemented yet");
    }
}