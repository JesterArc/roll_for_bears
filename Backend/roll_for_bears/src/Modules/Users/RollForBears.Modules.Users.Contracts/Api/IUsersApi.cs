using RollForBears.Modules.Users.Contracts.DTOs;

namespace RollForBears.Modules.Users.Contracts.Api;

public interface IUsersApi
{
    Task RegisterAsync(RegisterRequestDto request, CancellationToken cancellationToken);
    
    Task<LoginResultDto?> LoginAsync(LoginRequestDto request, CancellationToken cancellationToken);
    
    Task<LoginResultDto?> RefreshTokenAsync(string tokenRequest, CancellationToken cancellationToken);
    
    Task LogoutAsync(string tokenRequest, CancellationToken cancellationToken);
    Task<string?> GetPlayerUsernameAsync(Guid playerUuid, CancellationToken cancellationToken);
    Task SendFriendInviteAsync(Guid userUuid, Guid friendUuid, CancellationToken cancellationToken);
    Task AcceptFriendRequestAsync(Guid userUuid, Guid friendUuid, CancellationToken cancellationToken);
    Task RejectFriendAsync(Guid userUuid, Guid friendUuid, CancellationToken cancellationToken);
    Task<FriendRequestsResultDto> GetFriendRequestsAsync(Guid userUuid, CancellationToken cancellationToken);
    Task<ICollection<FriendDto>> GetFriendsAsync(Guid userUuid, CancellationToken cancellationToken);
    Task<Dictionary<Guid, string>> GetUsernamesAsync(List<Guid> userUuids, CancellationToken cancellationToken);
}