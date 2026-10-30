
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

    public async Task RegisterAsync(RegisterRequestDto request, CancellationToken cancellationToken)
    {
        bool emailExists = await _dbContext.Accounts.AnyAsync(a => a.Email == request.Email, cancellationToken);

        if (emailExists)
        {
            throw new InvalidOperationException("Email already exists");
        }
        
        bool usernameExist = await _dbContext.Accounts.AnyAsync(a => a.Username == request.Username, cancellationToken);

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
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<LoginResultDto?> LoginAsync(LoginRequestDto request, CancellationToken cancellationToken)
    {
        var account = await _dbContext.Accounts.AsNoTracking()
            .SingleOrDefaultAsync(a => a.Username == request.Username, cancellationToken);

        if (account == null)
            return null;

        bool passwordCorrect = _passwordHasher.Verify(request.Password, account.HashWord);
        
        if (!passwordCorrect)
            return null;
        
        string jwtToken = _jwtTokenGenerator.Generate(account.Uuid, account.Username!);

        string refreshToken = _refreshTokenGenerator.Generate();
        
        await AddRefreshTokenToDb(refreshToken, account.Uuid, cancellationToken);
        
        return new LoginResultDto(account.Uuid, account.Username!, jwtToken, refreshToken);
    }

    public async Task<LoginResultDto?> RefreshTokenAsync(string tokenRequest, CancellationToken cancellationToken)
    {
        string hash = _refreshTokenGenerator.Hash(tokenRequest);
        
        var refreshToken = await _dbContext.RefreshTokens.Include(rt => rt.Account)
            .SingleOrDefaultAsync(rt => rt.TokenHash == hash, cancellationToken);
        DateTime currentTime = DateTime.UtcNow;
        if (refreshToken == null)
        {
            return null;
        }

        if (refreshToken.RevokedAt != null)
        {
            const string querry = """
                                  UPDATE user_info.refresh_token
                                  SET revoked_at = @revokedAt
                                  WHERE family_uuid = @familyId
                                  AND revoked_at IS NULL;
                                  """;
            
            var revokedParam = new NpgsqlParameter("revokedAt", currentTime);
            var familyParam = new NpgsqlParameter("familyId", refreshToken.FamilyId);
            
            await _dbContext.Database.ExecuteSqlRawAsync(querry, revokedParam, familyParam, cancellationToken);

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
        
        await _dbContext.RefreshTokens.AddAsync(refreshTokenRecord, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
        
        var newLoginDto = new LoginResultDto(refreshToken.AccountId, refreshToken.Account.Username!,
            newJwtToken, newRefreshToken);
        
        return newLoginDto;
    }

    public async Task LogoutAsync(string tokenRequest, CancellationToken cancellationToken)
    {
        string tokenHash = _refreshTokenGenerator.Hash(tokenRequest);
        
        DateTime currentTime = DateTime.UtcNow;

        const string querry = """
                              UPDATE user_info.refresh_token
                              SET revoked_at = @revokedAt
                              WHERE family_uuid = (
                                  SELECT t.family_uuid FROM user_info.refresh_token t
                                                    WHERE t.token_hash = @hash
                                                    LIMIT 1)
                              AND revoked_at IS NULL;
                              """;
        
        var revokedParam = new NpgsqlParameter("revokedAt", currentTime);
        var hashParam = new NpgsqlParameter("hash", tokenHash);
        
        await _dbContext.Database.ExecuteSqlRawAsync(querry, revokedParam, hashParam, cancellationToken);
    }

    public async Task<string?> GetPlayerUsernameAsync(string playerUuid, CancellationToken cancellationToken)
    {
        return await _dbContext.Accounts.Where(a=> a.Uuid.ToString() == playerUuid).Select(a => a.Username)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<Dictionary<string, string>> GetSessionPlayersUsernamesAsync(List<string> playerUuids, CancellationToken cancellationToken)
    {
        var accounts = await _dbContext.Accounts.Where(a => playerUuids.Contains(a.Uuid.ToString()))
            .ToListAsync(cancellationToken);
        return accounts.ToDictionary(a => a.Uuid.ToString(), a => a.Username ?? "");
    }

    public async Task<Dictionary<string, string>> GetJoinRequestsUsernamesAsync(List<string> requestUuids, CancellationToken cancellationToken)
    {
        var accounts = await _dbContext.Accounts.Where(a => requestUuids.Contains(a.Uuid.ToString()))
            .ToListAsync(cancellationToken);
        return accounts.ToDictionary(a => a.Uuid.ToString(), a => a.Username ?? "");
    }

    private async Task AddRefreshTokenToDb(string token, Guid accountId, CancellationToken cancellationToken)
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
        
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}