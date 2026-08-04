namespace roll_for_bears.MainModules.Services;

public interface IUserValidateService
{
    public Task<bool> IsUsernameValidAsync(string username);
}