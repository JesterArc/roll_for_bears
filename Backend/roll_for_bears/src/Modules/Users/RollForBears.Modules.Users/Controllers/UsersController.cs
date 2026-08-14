using Microsoft.AspNetCore.Mvc;
using RollForBears.Modules.Users.Contracts.Api;
using RollForBears.Modules.Users.Contracts.DTOs;

namespace RollForBears.Modules.Users.Controllers;

[ApiController]
[Route("api/users")]
public sealed class UsersController : ControllerBase
{
    private readonly IUsersApi _usersApi;

    public UsersController(IUsersApi usersApi)
    {
        _usersApi = usersApi;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<AccountDto>>> GetAccounts()
    {
        var accounts = await _usersApi.GetAccountsAsync();

        return Ok(accounts);
    }
}