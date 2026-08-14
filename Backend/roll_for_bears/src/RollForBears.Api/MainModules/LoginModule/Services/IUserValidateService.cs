using roll_for_bears.MainModules.LoginModule.Models;

namespace roll_for_bears.MainModules.LoginModule.Services;

public interface IUserValidateService
{
    public Task<bool> IsUsernameValidAsync(string username);
    public Task<List<Account>> AccountsAsync();
}