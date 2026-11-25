using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using RollForBears.Modules.RpgContent.Contracts.Api;
using RollForBears.Modules.RpgContent.Contracts.DTOs;
using RollForBears.Modules.Shared_Kernel.Exceptions;

namespace RollForBears.Modules.RpgContent.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RpgContentController : ControllerBase
{
    private readonly IRpgContentApi _rpgContentApi;

    public RpgContentController(IRpgContentApi rpgContentApi)
    {
        _rpgContentApi = rpgContentApi;
    }

    [HttpGet("game-systems")]
    public async Task<IActionResult> GetGameSystems(CancellationToken cancellationToken)
    {
        var result = await _rpgContentApi.GetGameSystemAsync(cancellationToken);
        
        return Ok(result);
    }

    [HttpPost("game-systems")]
    public async Task<ActionResult<GameSystemDto>> CreateGameSystem(
        [FromBody] CreateGameSystemRequestDto request, CancellationToken cancellationToken)
    {
        var settings = request.Settings;

        if (settings.HasValue && settings.Value.ValueKind != JsonValueKind.Object)
        {
            throw new BadRequestException("Settings must be a JSON object.");
        }
        
        var result = await _rpgContentApi.CreateGameSystemAsync(request, cancellationToken);
        return Ok(result);
    }
}