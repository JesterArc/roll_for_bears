using Microsoft.EntityFrameworkCore;
using RollForBears.Modules.Sessions.Contracts.Api;
using RollForBears.Modules.Sessions.Contracts.DTOs;
using RollForBears.Modules.Sessions.Contracts.Enums;
using RollForBears.Modules.Sessions.Database;
using RollForBears.Modules.Users.Contracts.Api;

namespace RollForBears.Modules.Sessions.Services;

public class SessionsService : ISessionsApi
{
    private readonly SessionsDbContext _dbContext;
    //private readonly IUsersApi _userApi;

    public SessionsService(SessionsDbContext dbContext, IUsersApi userApi)
    {
        _dbContext = dbContext;
        //_userApi = userApi;
    }
    
    public async Task CreateSessionAsync(SessionCreateRequestDto request)
    {
        throw new NotImplementedException();
    }

    public async Task<SessionInfoDto?> GetSessionInfoAsync(string sessionId)
    {
        throw new NotImplementedException();
    }

    public async Task<ICollection<SessionInfoDto?>> GetSessionsAsync(int page, int pageSize)
    {
        var sessions = new List<SessionInfoDto?>();
        await _dbContext.Sessions.Skip((page) * pageSize).Take(pageSize).ForEachAsync(
            s =>
            {
                var session = new SessionInfoDto()
                {
                    Uuid = s.Uuid,
                    Name = s.Name,
                    Tagline = s.Tagline,
                    IsGeneratingNotes = s.IsGeneratingNotes,
                    GameSystem = s.GameSystem,
                    GameType = (s.GameType),
                    MinimalPlayers = s.MinimalPlayers,
                    MaximalPlayers = s.MaximalPlayers,
                    GameLanguage = s.GameLanguage,
                    OwnerUsername = "cannot find user",
                    SessionDescription = s.SessionDescription,
                    NextSessionDate = s.NextSessionDate,
                    Players = new List<SessionPlayerInfoDto>()
                };
                session.Players = s.Players.Select(p => new SessionPlayerInfoDto()
                {
                    Username = "cannot find user",
                    Role = p.Role
                }).ToList();
                sessions.Add(session);
            }
        );
        return sessions;
    }

    public async Task<ICollection<SessionInfoDto>> GetSessionsFilteredAsync(int page, int pageSize, SessionInfoFilteredRequestDto request)
    {
        throw new NotImplementedException();
    }

    public async Task<ICollection<SessionPlayerInfoDto>> GetSessionPlayerListAsync(string sessionId)
    {
        throw new NotImplementedException();
    }

    public async Task AddPlayerToSessionAsync(string sessionId, string username)
    {
        throw new NotImplementedException();
    }

    public async Task AddPlayerToSessionAsync(string sessionId, string username, PlayerRole playerRole)
    {
        throw new NotImplementedException();
    }

    public async Task ChangePlayerRoleAsync(string sessionId, string username, PlayerRole playerRole)
    {
        throw new NotImplementedException();
    }

    public async Task RemovePlayerAsync(string sessionId, string username)
    {
        throw new NotImplementedException();
    }

    public async Task<SessionInfoDto?> UpdateSessionInfoAsync(SessionUpdateRequestDto request)
    {
        throw new NotImplementedException();
    }

    public async Task DeleteSessionAsync(string sessionId)
    {
        throw new NotImplementedException();
    }
}