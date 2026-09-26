using RollForBears.Modules.Sessions.Contracts.DTOs;
using RollForBears.Modules.Sessions.Contracts.Enums;

namespace RollForBears.Modules.Sessions.Contracts.Api;

public interface ISessionsApi
{
    Task CreateSessionAsync(SessionCreateRequestDto request);
    
    Task<SessionInfoDto?> GetSessionInfoAsync(string sessionId);
    
    Task<ICollection<SessionInfoDto?>> GetSessionsAsync(int page, int pageSize);
    
    Task<ICollection<SessionInfoDto>> GetSessionsFilteredAsync(int page, int pageSize, SessionInfoFilteredRequestDto request);
    
    Task<ICollection<SessionPlayerInfoDto>> GetSessionPlayerListAsync(string sessionId);
    
    Task AddPlayerToSessionAsync(string sessionId, string username);
    Task AddPlayerToSessionAsync(string sessionId, string username, PlayerRole playerRole);
    
    Task ChangePlayerRoleAsync(string sessionId, string username, PlayerRole playerRole);
    
    Task RemovePlayerAsync(string sessionId, string username);
    
    Task<SessionInfoDto?> UpdateSessionInfoAsync(SessionUpdateRequestDto request);
    
    Task DeleteSessionAsync(string sessionId);
}