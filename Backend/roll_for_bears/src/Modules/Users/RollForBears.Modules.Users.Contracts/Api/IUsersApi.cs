using RollForBears.Modules.Users.Contracts.DTOs;

namespace RollForBears.Modules.Users.Contracts.Api;

public interface IUsersApi
{
    Task<IReadOnlyList<AccountDto>> GetAccountsAsync();
}