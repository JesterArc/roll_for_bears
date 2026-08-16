using RollForBears.Api.MainModules.LoginModule.Models;

namespace RollForBears.Api.MainModules.LoginModule.Services;

public interface IUserValidateService
{
    public Task<bool> IsUsernameValidAsync(string username);
    public Task<List<Account>> AccountsAsync();
}