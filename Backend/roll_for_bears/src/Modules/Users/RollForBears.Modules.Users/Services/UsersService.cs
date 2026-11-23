using Microsoft.EntityFrameworkCore;
using RollForBears.Modules.Shared_Kernel.Exceptions;
using RollForBears.Modules.Users.Contracts.Api;
using RollForBears.Modules.Users.Contracts.DTOs;
using RollForBears.Modules.Users.Contracts.Enums;
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
        var emailExists = await _dbContext.Accounts.AnyAsync(a => a.Email == request.Email, cancellationToken);

        if (emailExists)
        {
            throw new InvalidOperationException("Email already exists");
        }
        
        var usernameExist = await _dbContext.Accounts.AnyAsync(a => a.Username == request.Username, cancellationToken);

        if (usernameExist)
        {
            throw new InvalidOperationException("Username already exists");
        }

        var hash = _passwordHasher.Hash(request.Password);

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
        
        await _dbContext.Accounts.AddAsync(account, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<LoginResultDto?> LoginAsync(LoginRequestDto request, CancellationToken cancellationToken)
    {
        var account = await _dbContext.Accounts.AsNoTracking()
            .SingleOrDefaultAsync(a => a.Username == request.Username, cancellationToken);

        if (account == null)
            return null;

        var passwordCorrect = _passwordHasher.Verify(request.Password, account.HashWord);
        
        if (!passwordCorrect)
            return null;
        
        var jwtToken = _jwtTokenGenerator.Generate(account.Uuid, account.Username!);

        var refreshToken = _refreshTokenGenerator.Generate();
        
        await AddRefreshTokenToDb(refreshToken, account.Uuid, cancellationToken);
        
        return new LoginResultDto(account.Uuid, account.Username!, jwtToken, refreshToken);
    }

    public async Task<LoginResultDto?> RefreshTokenAsync(string tokenRequest, CancellationToken cancellationToken)
    {
        var hash = _refreshTokenGenerator.Hash(tokenRequest);
        
        var refreshToken = await _dbContext.RefreshTokens.Include(rt => rt.Account)
            .SingleOrDefaultAsync(rt => rt.TokenHash == hash, cancellationToken);
        
        var currentTime = DateTime.UtcNow;
        if (refreshToken == null)
        {
            return null;
        }

        if (refreshToken.RevokedAt != null)
        {
            var listOfTokens = await _dbContext.RefreshTokens
                .Where(rt => rt.FamilyId == refreshToken.FamilyId && rt.RevokedAt == null)
                .ToListAsync(cancellationToken);

            foreach (var token in listOfTokens)
            {
                token.RevokedAt = currentTime;
            }
            
            await _dbContext.SaveChangesAsync(cancellationToken);

            return null;
        }

        if (refreshToken.ExpiresAt <= DateTime.UtcNow)
        {
            return null;
        }
        currentTime = DateTime.UtcNow;
        
        var newJwtToken = _jwtTokenGenerator.Generate(refreshToken.AccountId, refreshToken.Account.Username!);
        var newRefreshToken = _refreshTokenGenerator.Generate();
        var newRTHash = _refreshTokenGenerator.Hash(newRefreshToken);

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
        var tokenHash = _refreshTokenGenerator.Hash(tokenRequest);
        
        var familyUuid = await _dbContext.RefreshTokens
            .Where(rt => rt.TokenHash == tokenHash)
            .Select(rt => rt.FamilyId).FirstOrDefaultAsync(cancellationToken);
        
        var refreshToken = await _dbContext.RefreshTokens
            .Where(rt => rt.FamilyId == familyUuid && rt.RevokedAt == null)
            .FirstOrDefaultAsync(cancellationToken);

        if (refreshToken != null)
        {
            refreshToken.RevokedAt = DateTime.UtcNow;
        }
        
        await _dbContext.SaveChangesAsync(cancellationToken);
        
    }

    public async Task<string?> GetPlayerUsernameAsync(Guid playerUuid, CancellationToken cancellationToken)
    {
        return await _dbContext.Accounts.AsNoTracking()
            .Where(a=> a.Uuid == playerUuid).Select(a => a.Username)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task SendFriendInviteAsync(Guid userUuid, Guid friendUuid, CancellationToken cancellationToken)
    {
        await CheckIfUserAndFriendValid(userUuid, friendUuid, cancellationToken);

        var alreadyFriends = await _dbContext.Friends.AsNoTracking()
            .AnyAsync(fr => (fr.FriendUuid == friendUuid && fr.UserUuid == userUuid) ||
                            (fr.FriendUuid == userUuid && fr.UserUuid == friendUuid), cancellationToken);

        if (alreadyFriends)
        {
            throw new BadRequestException("Users are already friends or a friend request was already sent");
        }

        await _dbContext.Friends.AddAsync(
            new Friend
            {
                FriendshipUuid = Guid.NewGuid(),
                UserUuid = userUuid,
                FriendUuid = friendUuid,
                FriendshipMutual = false
            }, cancellationToken);
        
        Console.Write($"user: {userUuid}, friendship: {friendUuid}");
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task AcceptFriendRequestAsync(Guid userUuid, Guid friendUuid, CancellationToken cancellationToken)
    {
        await CheckIfUserAndFriendValid(userUuid, friendUuid, cancellationToken);

        var friendRequest = await _dbContext.Friends
            .SingleOrDefaultAsync(fr => (fr.FriendUuid == friendUuid && fr.UserUuid == userUuid) || 
                                        (fr.FriendUuid == userUuid && fr.UserUuid == friendUuid), cancellationToken);

        if (friendRequest == null)
        {
            throw new BadRequestException("Friend request not found");
        }

        if (friendRequest.UserUuid == userUuid)
        {
            throw new BadRequestException("User cannot accept friend request they sent");
        }
        
        if (friendRequest.FriendshipMutual)
        {
            throw new BadRequestException("Users are already friends");
        }
        
        friendRequest.FriendshipMutual = true;
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
    public async Task RejectFriendAsync(Guid userUuid, Guid friendUuid, CancellationToken cancellationToken)
    {
        await CheckIfUserAndFriendValid(userUuid, friendUuid, cancellationToken);
        
        var friendRequest = await _dbContext.Friends
            .SingleOrDefaultAsync(fr => (fr.FriendUuid == friendUuid && fr.UserUuid == userUuid) || 
                                        (fr.FriendUuid == userUuid && fr.UserUuid == friendUuid), cancellationToken);

        if (friendRequest == null)
        {
            throw new BadRequestException("Friend request not found");
        }
        
        _dbContext.Friends.Remove(friendRequest);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
    public async Task<FriendRequestsResultDto> GetFriendRequestsAsync(Guid userUuid, CancellationToken cancellationToken)
    {
        var userExists = await _dbContext.Accounts.AsNoTracking()
            .AnyAsync(a => a.Uuid.Equals(userUuid), cancellationToken);
        
        if (!userExists)
        {
            throw new NotFoundException("User not found");
        }
        
        var friendRequests = await _dbContext.Friends.AsNoTracking()
            .Where(fr => (fr.UserUuid == userUuid || fr.FriendUuid == userUuid) && !fr.FriendshipMutual)
            .ToListAsync(cancellationToken);
        
        var usernames = await GetUsernamesAsync(friendRequests
            .Select(fr => fr.FriendUuid == userUuid ? userUuid : fr.FriendUuid)
            .ToList(), cancellationToken);
        
        var friendRequestsIncoming = new List<FriendDto>();
        var friendRequestsOutgoing = new List<FriendDto>();
        
        friendRequests.ForEach(fr =>
        {
            if (fr.FriendUuid == userUuid)
            {
                friendRequestsIncoming.Add(new FriendDto { Username = usernames[fr.UserUuid], FriendshipMutual = false });
            }
            else
            {
                friendRequestsOutgoing.Add(new FriendDto { Username = usernames[fr.FriendUuid], FriendshipMutual = false });
            }
        });

        return new FriendRequestsResultDto
        {
            IncomingFriendRequests = friendRequestsIncoming,
            OutgoingFriendRequests = friendRequestsOutgoing
        };
    }
    public async Task<ICollection<FriendDto>> GetFriendsAsync(Guid userUuid, CancellationToken cancellationToken)
    {
        var userExists = await _dbContext.Accounts.AsNoTracking()
            .AnyAsync(a => a.Uuid.Equals(userUuid), cancellationToken);
        
        if (!userExists)
        {
            throw new NotFoundException("User not found");
        }
        
        var friends = await _dbContext.Friends.AsNoTracking()
            .Where(fr => (fr.FriendUuid == userUuid || fr.UserUuid == userUuid) && fr.FriendshipMutual)
            .ToListAsync(cancellationToken);
        
        var usernames = await GetUsernamesAsync(friends
            .Select(fr => fr.FriendUuid == userUuid ? userUuid : fr.FriendUuid)
            .ToList(), cancellationToken);
        
        return friends.Select(fr => new FriendDto
        {
            Username = usernames[fr.FriendUuid == userUuid ? userUuid : fr.FriendUuid],
            FriendshipMutual = true
        }).ToList();
    }

    public async Task<Dictionary<Guid, string>> GetUsernamesAsync(List<Guid> userUuids, CancellationToken cancellationToken)
    {
        var accounts = await _dbContext.Accounts.AsNoTracking()
            .Where(a => userUuids.Contains(a.Uuid))
            .ToListAsync(cancellationToken);
        return accounts.ToDictionary(a => a.Uuid, a => a.Username ?? "");
    }

    private async Task AddRefreshTokenToDb(string token, Guid accountId, CancellationToken cancellationToken)
    {
        var hash = _refreshTokenGenerator.Hash(token);
        var currentTime = DateTime.UtcNow;
        
        var refreshToken = new RefreshToken
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
        
        await _dbContext.RefreshTokens.AddAsync(refreshToken, cancellationToken);
        
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    private async Task CheckIfUserAndFriendValid(Guid userUuid, Guid friendUuid, CancellationToken cancellationToken)
    {
        if (userUuid == friendUuid)
        {
            throw new BadRequestException("User and friend cannot have the same uuid");
        }
        
        var userExists = await _dbContext.Accounts.AsNoTracking()
            .AnyAsync(a => a.Uuid.Equals(userUuid), cancellationToken);
        
        if (!userExists)
        {
            throw new NotFoundException("User not found");
        }
        
        var friendExists = await _dbContext.Accounts.AsNoTracking()
            .AnyAsync(a => a.Uuid.Equals(friendUuid), cancellationToken);
        
        if (!friendExists)
        {
            throw new NotFoundException("Friend not found");
        }
    }
}