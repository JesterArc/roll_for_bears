using RollForBears.Modules.Sessions.Contracts.DTOs;

namespace RollForBears.Modules.Sessions.Contracts.Api;

public interface ISessionsApi
{
    Task<Guid> CreateSessionAsync(SessionCreateRequestDto request, Guid ownerUuid, CancellationToken cancellationToken);
    
    Task<SessionInfoSpecificDto?> GetSpecificSessionInfoAsync(Guid sessionUuid, 
        CancellationToken cancellationToken);
    
    Task<ICollection<SessionInfoListViewDto>> GetSessionsAsync(int page, int pageSize, 
        CancellationToken cancellationToken);
    
    Task<ICollection<SessionInfoListViewDto>> GetSessionsFilteredAsync(int page, int pageSize, 
        SessionFilteredRequestDto request, CancellationToken cancellationToken);
    
    Task<ICollection<SessionPlayerInfoDto>> GetSessionPlayerListAsync(Guid sessionUuid, 
        CancellationToken cancellationToken);
    
    Task<ICollection<SessionJoinRequestInfoDto>> GetSessionJoinRequestListAsync(Guid sessionUuid, 
        CancellationToken cancellationToken);
    
    Task<Guid> AddPlayerToSessionAsync(Guid sessionUuid, SessionPlayerDto request, Guid userUuid, CancellationToken cancellationToken);
    
    Task<Guid> ChangePlayerRoleAsync(Guid sessionUuid, SessionPlayerDto request, Guid userUuid, CancellationToken cancellationToken);
    
    Task<Guid> RemovePlayerAsync(SessionPlayerRemoveDto request, Guid userUuid, CancellationToken cancellationToken);
    
    Task<Guid> UpdateSessionInfoAsync(Guid sessionUuid, SessionUpdateRequestDto request, Guid userUuid, 
        CancellationToken cancellationToken);
    
    Task DeleteSessionAsync(Guid sessionUuid, Guid userUuid, CancellationToken cancellationToken);
    Task<Guid> AddJoinRequestToSessionAsync(Guid sessionUuid, SessionJoinRequestRoleDto request, Guid userUuid, CancellationToken cancellationToken);
    Task<Guid> RemoveJoinRequestFromSessionAsync(SessionJoinRequestRemoveDto request, Guid userUuid, CancellationToken cancellationToken);
}