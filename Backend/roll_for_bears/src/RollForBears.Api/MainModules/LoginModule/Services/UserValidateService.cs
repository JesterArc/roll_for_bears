using Microsoft.EntityFrameworkCore;
using roll_for_bears.Database;
using RollForBears.Api.MainModules.LoginModule.Models;

namespace RollForBears.Api.MainModules.LoginModule.Services;

public class UserValidateService : IUserValidateService
{
    private readonly RollForBearsContext _context;
    public UserValidateService(RollForBearsContext context)
    {
        _context = context;
    }
    public async Task<bool> IsUsernameValidAsync(string username)
    {
        var isUsernameValid = Task.FromResult(username.Length is >= 6 and <= 20);
        return await isUsernameValid;
    }

    public async Task<List<Account>> AccountsAsync()
    {
        var accounts = await _context.Accounts.ToListAsync();
        return await Task.FromResult(accounts);
    }
}