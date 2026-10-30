using RollForBears.Modules.Users.Contracts.DTOs;

namespace RollForBears.Modules.Users.Contracts.Api;

public interface IUsersApi
{
    Task RegisterAsync(RegisterRequestDto request, CancellationToken cancellationToken);
    
    Task<LoginResultDto?> LoginAsync(LoginRequestDto request, CancellationToken cancellationToken);
    
    Task<LoginResultDto?> RefreshTokenAsync(string tokenRequest, CancellationToken cancellationToken);
    
    Task LogoutAsync(string tokenRequest, CancellationToken cancellationToken);
    Task<string?> GetPlayerUsernameAsync(string playerUuid, CancellationToken cancellationToken);
    
    Task<Dictionary<string, string>> GetSessionPlayersUsernamesAsync(List<string> playerUuids, CancellationToken cancellationToken);
    Task<Dictionary<string, string>> GetJoinRequestsUsernamesAsync(List<string> requestUuids, CancellationToken cancellationToken);
}