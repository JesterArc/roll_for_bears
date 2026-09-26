using RollForBears.Modules.Sessions.Contracts.Api;
using RollForBears.Modules.Sessions.Contracts.DTOs;
using RollForBears.Modules.Sessions.Contracts.Enums;

namespace RollForBears.Modules.Sessions.Services;

public class SessionsService : ISessionsApi
{
    public Task CreateSessionAsync(SessionCreateRequestDto request)
    {
        throw new NotImplementedException();
    }

    public Task<SessionInfoDto?> GetSessionInfoAsync(string sessionId)
    {
        throw new NotImplementedException();
    }

    public Task<ICollection<SessionInfoDto?>> GetSessionsAsync(int page, int pageSize)
    {
        throw new NotImplementedException();
    }

    public Task<ICollection<SessionInfoDto>> GetSessionsFilteredAsync(int page, int pageSize, SessionInfoFilteredRequestDto request)
    {
        throw new NotImplementedException();
    }

    public Task<ICollection<SessionPlayerInfoDto>> GetSessionPlayerListAsync(string sessionId)
    {
        throw new NotImplementedException();
    }

    public Task AddPlayerToSessionAsync(string sessionId, string username)
    {
        throw new NotImplementedException();
    }

    public Task AddPlayerToSessionAsync(string sessionId, string username, PlayerRole playerRole)
    {
        throw new NotImplementedException();
    }

    public Task ChangePlayerRoleAsync(string sessionId, string username, PlayerRole playerRole)
    {
        throw new NotImplementedException();
    }

    public Task RemovePlayerAsync(string sessionId, string username)
    {
        throw new NotImplementedException();
    }

    public Task<SessionInfoDto?> UpdateSessionInfoAsync(SessionUpdateRequestDto request)
    {
        throw new NotImplementedException();
    }

    public Task DeleteSessionAsync(string sessionId)
    {
        throw new NotImplementedException();
    }
}