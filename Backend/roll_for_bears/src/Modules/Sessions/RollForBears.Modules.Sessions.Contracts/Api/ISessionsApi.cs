using RollForBears.Modules.Sessions.Contracts.DTOs;
using RollForBears.Modules.Sessions.Contracts.Enums;

namespace RollForBears.Modules.Sessions.Contracts.Api;

public interface ISessionsApi
{
    Task<Guid> CreateSessionAsync(SessionCreateRequestDto request, CancellationToken cancellationToken);
    
    Task<SessionInfoSpecificDto?> GetSessionInfoAsync(string sessionId, CancellationToken cancellationToken);
    Task<SessionInfoSpecificDto?> GetSpecificSessionInfoAsync(string sessionId, CancellationToken cancellationToken);
    
    Task<ICollection<SessionInfoListViewDto?>> GetSessionsAsync(int page, int pageSize, CancellationToken cancellationToken);
    
    Task<ICollection<SessionInfoListViewDto>> GetSessionsFilteredAsync(int page, int pageSize, SessionInfoFilteredRequestDto request, CancellationToken cancellationToken);
    
    Task<ICollection<SessionPlayerInfoDto>> GetSessionPlayerListAsync(string sessionId, CancellationToken cancellationToken);
    
    Task<Guid> AddPlayerToSessionAsync(SessionPlayerAddDto dto, CancellationToken cancellationToken);
    
    Task<Guid> ChangePlayerRoleAsync(SessionPlayerAddDto dto, CancellationToken cancellationToken);
    
    Task<Guid> RemovePlayerAsync(SessionPlayerRemoveDto dto, CancellationToken cancellationToken);
    
    Task<SessionInfoSpecificDto?> UpdateSessionInfoAsync(SessionUpdateRequestDto request, CancellationToken cancellationToken);
    
    Task DeleteSessionAsync(string sessionId, CancellationToken cancellationToken);
}