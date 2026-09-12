
using Microsoft.EntityFrameworkCore;
using Npgsql;
using RollForBears.Modules.Users.Contracts.Api;
using RollForBears.Modules.Users.Contracts.DTOs;
using RollForBears.Modules.Users.Database;
using RollForBears.Modules.Users.Models;
using RollForBears.Modules.Users.Security;

namespace RollForBears.Modules.Users.Services;

internal sealed class UsersService : IUsersApi
{
    private readonly UsersDbContext _dbContext;
    
    private readonly PasswordHasher _passwordHasher;
    
    private readonly JwtTokenGenerator _jwtTokenGenerator;
    
    private readonly RefreshTokenGenerator _refreshTokenGenerator;

    public UsersService(UsersDbContext dbContext, PasswordHasher passwordHasher, JwtTokenGenerator jwtTokenGenerator, RefreshTokenGenerator refreshTokenGenerator)
    {
        _dbContext = dbContext;
        _passwordHasher = passwordHasher;
        _jwtTokenGenerator = jwtTokenGenerator;
        _refreshTokenGenerator = refreshTokenGenerator;
    }

    public async Task RegisterAsync(RegisterRequestDto request)
    {
        bool emailExists = await _dbContext.Accounts.AnyAsync(a => a.Email == request.Email);

        if (emailExists)
        {
            throw new InvalidOperationException("Email already exists");
        }
        
        bool usernameExist = await _dbContext.Accounts.AnyAsync(a => a.Username == request.Username);

        if (usernameExist)
        {
            throw new InvalidOperationException("Username already exists");
        }

        string hash = _passwordHasher.Hash(request.Password);

        var account = new Account
        {
            Uuid = Guid.NewGuid(),
            HashWord = hash,
            Email = request.Email,
            CreatedAt = DateOnly.FromDateTime(DateTime.UtcNow),
            StatusChangedAt = DateTime.UtcNow,
            Status = AccountStatus.Pending,
            Username = request.Username
        };
        _dbContext.Accounts.Add(account);
        await _dbContext.SaveChangesAsync();
    }

    public async Task<LoginResultDto?> LoginAsync(LoginRequestDto request)
    {
        var account = await _dbContext.Accounts.AsNoTracking()
            .SingleOrDefaultAsync(a => a.Username == request.Username);

        if (account == null)
            return null;

        bool passwordCorrect = _passwordHasher.Verify(request.Password, account.HashWord);
        
        if (!passwordCorrect)
            return null;
        
        string jwtToken = _jwtTokenGenerator.Generate(account.Uuid, account.Username!);

        string refreshToken = _refreshTokenGenerator.Generate();
        
        await AddRefreshTokenToDb(refreshToken, account.Uuid);
        
        return new LoginResultDto(account.Uuid, account.Username!, jwtToken, refreshToken);
    }

    public async Task<LoginResultDto?> RefreshTokenAsync(string tokenRequest)
    {
        string hash = _refreshTokenGenerator.Hash(tokenRequest);
        
        var refreshToken = await _dbContext.RefreshTokens.Include(rt => rt.Account)
            .SingleOrDefaultAsync(rt => rt.TokenHash == hash);
        DateTime currentTime = DateTime.UtcNow;
        if (refreshToken == null)
        {
            return null;
        }

        if (refreshToken.RevokedAt != null)
        {
            const string querry = """
                                  UPDATE user_info.refresh_token
                                  SET "RevokedAt" = @revokedAt
                                  WHERE "FmailyId" = @familyId
                                  AND "RevokedAt" IS NULL;
                                  """;
            
            var revokedParam = new NpgsqlParameter("revokedAt", currentTime);
            var familyParam = new NpgsqlParameter("familyId", refreshToken.FamilyId);
            
            await _dbContext.Database.ExecuteSqlRawAsync(querry, revokedParam, familyParam);

            return null;
        }

        if (refreshToken.ExpiresAt <= DateTime.UtcNow)
        {
            return null;
        }

        currentTime = DateTime.UtcNow;
        string newJwtToken = _jwtTokenGenerator.Generate(refreshToken.AccountId, refreshToken.Account.Username!);
        string newRefreshToken = _refreshTokenGenerator.Generate();
        string newRTHash = _refreshTokenGenerator.Hash(newRefreshToken);

        var refreshTokenRecord = new RefreshToken
        {
            Uuid = Guid.NewGuid(),
            AccountId = refreshToken.AccountId,
            TokenHash = newRTHash,
            CreatedAt = currentTime,
            ExpiresAt = currentTime.AddHours(48),
            RevokedAt = null,
            ReplacedByToken = null,
            FamilyId = refreshToken.FamilyId
        };
        
        refreshToken.RevokedAt = currentTime;
        refreshToken.ReplacedByTokenId = refreshTokenRecord.Uuid;
        
        _dbContext.RefreshTokens.Add(refreshTokenRecord);
        await _dbContext.SaveChangesAsync();
        
        var newLoginDto = new LoginResultDto(refreshToken.AccountId, refreshToken.Account.Username!,
            newJwtToken, newRefreshToken);
        
        return newLoginDto;
    }

    public async Task LogoutAsync(string tokenRequest)
    {
        string tokenHash = _refreshTokenGenerator.Hash(tokenRequest);
        
        DateTime currentTime = DateTime.UtcNow;

        const string querry = """
                              UPDATE user_info.refresh_token
                              SET "RevokedAt" = @revokedAt
                              WHERE "FamilyId" = (
                                  SELECT "FamilyId" FROM user_info.refresh_token
                                                    WHERE "TokenHash" = @hash
                                                    LIMIT 1)
                              AND "RevokedAt" IS NULL;
                              """;
        
        var revokedParam = new NpgsqlParameter("revokedAt", currentTime);
        var hashParam = new NpgsqlParameter("hash", tokenHash);
        
        await _dbContext.Database.ExecuteSqlRawAsync(querry, revokedParam, hashParam);
    }

    private async Task AddRefreshTokenToDb(string token, Guid accountId)
    {
        string hash = _refreshTokenGenerator.Hash(token);
        
        DateTime currentTime = DateTime.UtcNow;
        var refreshToken = new RefreshToken()
        {
            Uuid = Guid.NewGuid(),
            AccountId = accountId,
            TokenHash = hash,
            FamilyId = Guid.NewGuid(),
            CreatedAt = currentTime,
            ExpiresAt = currentTime.AddHours(48),
            RevokedAt = null,
            ReplacedByToken = null
        };
        
        _dbContext.RefreshTokens.Add(refreshToken);
        
        await _dbContext.SaveChangesAsync();
    }
}