using Microsoft.AspNetCore.Mvc;
using roll_for_bears.MainModules.Services;

namespace roll_for_bears.MainModules.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UserValidateController(IUserValidateService userValidateService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetUsernameValid([FromQuery] string username)
    {
        if (!await userValidateService.IsUsernameValidAsync(username))
        {
            return BadRequest("Username is invalid");
        }
        return Ok($"Username {username} is valid");
    }
}