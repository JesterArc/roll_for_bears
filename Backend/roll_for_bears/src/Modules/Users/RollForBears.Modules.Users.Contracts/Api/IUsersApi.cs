using RollForBears.Modules.Users.Contracts.DTOs;

namespace RollForBears.Modules.Users.Contracts.Api;

public interface IUsersApi
{
    Task RegisterAsync(RegisterRequestDto request, CancellationToken cancellationToken);
    
    Task<LoginResultDto?> LoginAsync(LoginRequestDto request, CancellationToken cancellationToken);
    
    Task<LoginResultDto?> RefreshTokenAsync(string tokenRequest, CancellationToken cancellationToken);
    
    Task LogoutAsync(string tokenRequest, CancellationToken cancellationToken);
    Task<string?> GetPlayerUsernameAsync(Guid playerUuid, CancellationToken cancellationToken);
    Task<Dictionary<Guid, string>> GetUsernamesAsync(List<Guid> userUuids, CancellationToken cancellationToken);
    Task CheckIfUserExistsAsync(Guid userUuid, CancellationToken cancellationToken, string message="User not found");
}