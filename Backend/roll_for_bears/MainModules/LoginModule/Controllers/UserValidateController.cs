using Microsoft.AspNetCore.Mvc;
using roll_for_bears.Database;
using roll_for_bears.MainModules.LoginModule.Services;

namespace roll_for_bears.MainModules.LoginModule.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UserValidateController : ControllerBase
{

    private readonly IUserValidateService _userValidateService;
    public UserValidateController(IUserValidateService userValidateService, RollForBearsContext context)
    {
        _userValidateService = userValidateService;
    }
    
    [HttpGet]
    public async Task<IActionResult> GetAccounts()
    {
        return Ok(await _userValidateService.AccountsAsync());
    }
}