namespace roll_for_bears.MainModules.Services;

public class UserValidateService : IUserValidateService
{
    public async Task<bool> IsUsernameValidAsync(string username)
    {
        var isUsernameValid = Task.FromResult(username.Length is >= 6 and <= 20);
        return await isUsernameValid;
    }
}