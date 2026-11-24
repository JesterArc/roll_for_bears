using RollForBears.Modules.Users.Contracts.DTOs;

namespace RollForBears.Modules.Users.Contracts.Api;

public interface IFriendsApi
{
    Task SendFriendInviteAsync(Guid userUuid, Guid friendUuid, CancellationToken cancellationToken);
    Task AcceptFriendRequestAsync(Guid userUuid, Guid friendUuid, CancellationToken cancellationToken);
    Task RejectFriendAsync(Guid userUuid, Guid friendUuid, CancellationToken cancellationToken);
    Task<FriendRequestsResultDto> GetFriendRequestsAsync(Guid userUuid, CancellationToken cancellationToken);
    Task<ICollection<FriendDto>> GetFriendsAsync(Guid userUuid, CancellationToken cancellationToken);
}