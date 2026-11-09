using RollForBears.Modules.Sessions.Contracts.DTOs;

namespace RollForBears.Modules.Sessions.Contracts.Api;

public interface ISessionsApi
{
    Task<Guid> CreateSessionAsync(SessionCreateRequestDto request, Guid ownerUuid, CancellationToken cancellationToken);
    
    Task<SessionInfoSpecificDto?> GetSpecificSessionInfoAsync(string sessionId, 
        CancellationToken cancellationToken);
    
    Task<ICollection<SessionInfoListViewDto>> GetSessionsAsync(int page, int pageSize, 
        CancellationToken cancellationToken);
    
    Task<ICollection<SessionInfoListViewDto>> GetSessionsFilteredAsync(int page, int pageSize, 
        SessionFilteredRequestDto request, CancellationToken cancellationToken);
    
    Task<ICollection<SessionPlayerInfoDto>> GetSessionPlayerListAsync(string sessionId, 
        CancellationToken cancellationToken);
    
    Task<ICollection<SessionJoinRequestInfoDto>> GetSessionJoinRequestListAsync(string sessionId, 
        CancellationToken cancellationToken);
    
    Task<Guid> AddPlayerToSessionAsync(SessionPlayerAddDto request, Guid userUuid, CancellationToken cancellationToken);
    
    Task<Guid> ChangePlayerRoleAsync(SessionPlayerAddDto request, Guid userUuid, CancellationToken cancellationToken);
    
    Task<Guid> RemovePlayerAsync(SessionPlayerRemoveDto request, Guid userUuid, CancellationToken cancellationToken);
    
    Task<Guid> UpdateSessionInfoAsync(SessionUpdateRequestDto request, Guid userUuid, 
        CancellationToken cancellationToken);
    
    Task DeleteSessionAsync(string sessionId, Guid userUuid, CancellationToken cancellationToken);
    Task<Guid> AddJoinRequestToSessionAsync(SessionJoinRequestAddDto request, Guid userUuid, CancellationToken cancellationToken);
    Task<Guid> RemoveJoinRequestFromSessionAsync(SessionJoinRequestRemoveDto request, Guid userUuid, CancellationToken cancellationToken);
}