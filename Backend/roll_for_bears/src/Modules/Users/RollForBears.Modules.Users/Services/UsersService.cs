
using Microsoft.EntityFrameworkCore;
using RollForBears.Modules.Users.Contracts.Api;
using RollForBears.Modules.Users.Contracts.DTOs;
using RollForBears.Modules.Users.Database;

namespace RollForBears.Modules.Users.Services;

internal sealed class UsersService : IUsersApi
{
    private readonly UsersDbContext _dbContext;

    public UsersService(UsersDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyList<AccountDto>> GetAccountsAsync()
    {
        return await _dbContext.Accounts
            .AsNoTracking()
            .Select(account => new AccountDto(
                account.Uuid,
                account.Email,
                account.CreatedAt,
                account.StatusChangedAt
            ))
            .ToListAsync();
    }
    
}