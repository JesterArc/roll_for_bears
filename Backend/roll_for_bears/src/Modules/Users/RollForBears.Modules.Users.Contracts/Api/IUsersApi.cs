using RollForBears.Modules.Users.Contracts.DTOs;

namespace RollForBears.Modules.Users.Contracts.Api;

public interface IUsersApi
{
    Task RegisterAsync(RegisterRequestDto request);
    
    Task<LoginResultDto?> LoginAsync(LoginRequestDto request);
    
    Task<LoginResultDto?> RefreshTokenAsync(string tokenRequest);
    
    Task LogoutAsync(string tokenRequest);
}