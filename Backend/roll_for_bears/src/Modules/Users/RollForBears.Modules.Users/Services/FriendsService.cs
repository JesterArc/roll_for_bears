using Microsoft.EntityFrameworkCore;
using RollForBears.Modules.Shared_Kernel.Exceptions;
using RollForBears.Modules.Users.Contracts.Api;
using RollForBears.Modules.Users.Contracts.DTOs;
using RollForBears.Modules.Users.Database;
using RollForBears.Modules.Users.Models;

namespace RollForBears.Modules.Users.Services;

public class FriendsService : IFriendsApi
{
    private readonly UsersDbContext _dbContext;
    private readonly IUsersApi _userApi;

    public FriendsService(UsersDbContext dbContext, IUsersApi userApi)
    {
        _dbContext = dbContext;
        _userApi = userApi;
    }
    
    public async Task SendFriendInviteAsync(Guid userUuid, Guid friendUuid, CancellationToken cancellationToken)
    {
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
        await _userApi.CheckIfUserExistsAsync(userUuid, cancellationToken);
        
        var friendRequests = await _dbContext.Friends.AsNoTracking()
            .Where(fr => (fr.UserUuid == userUuid || fr.FriendUuid == userUuid) && !fr.FriendshipMutual)
            .ToListAsync(cancellationToken);
        
        var usernames = await _userApi.GetUsernamesAsync(friendRequests
            .Select(fr => fr.FriendUuid == userUuid ? fr.UserUuid : fr.FriendUuid)
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
        await _userApi.CheckIfUserExistsAsync(userUuid, cancellationToken);
        
        var friends = await _dbContext.Friends.AsNoTracking()
            .Where(fr => (fr.FriendUuid == userUuid || fr.UserUuid == userUuid) && fr.FriendshipMutual)
            .ToListAsync(cancellationToken);
        
        var usernames = await _userApi.GetUsernamesAsync(friends
            .Select(fr => fr.FriendUuid == userUuid ? fr.UserUuid : fr.FriendUuid)
            .ToList(), cancellationToken);
        
        return friends.Select(fr => new FriendDto
        {
            Username = usernames[fr.FriendUuid == userUuid ? fr.UserUuid : fr.FriendUuid],
            FriendshipMutual = true
        }).ToList();
    }

    private async Task CheckIfUserAndFriendValid(Guid userUuid, Guid friendUuid, CancellationToken cancellationToken)
    {
        if (userUuid == friendUuid)
        {
            throw new BadRequestException("User cannot be the same as friend");
        }
        await _userApi.CheckIfUserExistsAsync(userUuid, cancellationToken);
        await _userApi.CheckIfUserExistsAsync(friendUuid, cancellationToken, "Friend not found");
    }
}