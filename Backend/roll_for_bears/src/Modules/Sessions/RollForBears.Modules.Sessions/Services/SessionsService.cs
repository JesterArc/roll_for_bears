using Microsoft.EntityFrameworkCore;
using RollForBears.Modules.Sessions.Contracts.Api;
using RollForBears.Modules.Sessions.Contracts.DTOs;
using RollForBears.Modules.Sessions.Contracts.Enums;
using RollForBears.Modules.Sessions.Database;
using RollForBears.Modules.Sessions.Models;
using RollForBears.Modules.Users.Contracts.Api;
using static System.Enum;

namespace RollForBears.Modules.Sessions.Services;

public class SessionsService : ISessionsApi
{
    private readonly SessionsDbContext _dbContext;
    private readonly IUsersApi _userApi;

    public SessionsService(SessionsDbContext dbContext, IUsersApi userApi)
    {
        _dbContext = dbContext;
        _userApi = userApi;
    }
    
    public async Task<Guid> CreateSessionAsync(SessionCreateRequestDto request)
    {
        // throw new NotImplementedException();
        
        
        // TryParse(request.GameType, out GameType gameType);
        var newSession = new Session()
        {
            Uuid = Guid.NewGuid(),
            Name = request.Name,
            Tagline = request.Tagline,
            IsGeneratingNotes = request.IsGeneratingNotes,
            GameSystem = request.GameSystem,
            GameType = GameType.Campaign,
            MinimalPlayers = request.MinimalPlayers,
            MaximalPlayers = request.MaximalPlayers,
            GameLanguage = request.GameLanguage,
            OwnerUuid = request.OwnerUuid,
            SessionDescription = request.GameDescription,
            NextSessionDate = request.NextSessionDate
        };
        
        await _dbContext.AddAsync(newSession);
        await _dbContext.SaveChangesAsync();
        return newSession.Uuid;
    }

    public async Task<SessionInfoSpecificDto?> GetSessionInfoAsync(string sessionId)
    {
        throw new NotImplementedException();
    }

    public async Task<SessionInfoSpecificDto?> GetSpecificSessionInfoAsync(string sessionId)
    {
        var session = await _dbContext.Sessions.Where(s => s.Uuid.ToString() == sessionId)
            .FirstOrDefaultAsync();
        if (session == null)
        {
            return null;
        }
        var specificSession = new SessionInfoSpecificDto()
        {
            Uuid = session.Uuid,
            Name = session.Name,
            Tagline = session.Tagline,
            GameSystem = session.GameSystem,
            GameType = session.GameType.ToString(),
            OwnerUsername = "",
            MinimalPlayers = session.MinimalPlayers,
            MaximalPlayers = session.MaximalPlayers,
            GameLanguage = session.GameLanguage,
            NextSessionDate = session.NextSessionDate,
            SessionDescription = session.SessionDescription,
            Players = new List<SessionPlayerInfoDto>()
        };
        specificSession.OwnerUsername = await _userApi.GetPlayerUsernameAsync(session.OwnerUuid.ToString()) ?? "";
        specificSession.Players = await GetSessionPlayerListAsync(sessionId);
        return specificSession;
    }

    public async Task<ICollection<SessionInfoListViewDto?>> GetSessionsAsync(int page, int pageSize)
    {
        var sessions = new List<SessionInfoListViewDto?>();
        try
        {
            sessions = (await _dbContext.Sessions
                .Skip(page * pageSize)
                .Take(pageSize)
                .Select(s => new SessionInfoListViewDto()
                {
                    Uuid = s.Uuid,
                    Name = s.Name,
                    Tagline = s.Tagline,
                    GameSystem = s.GameSystem,
                    GameType = s.GameType.ToString(),
                    MaximalPlayers = s.MaximalPlayers,
                    GameLanguage = s.GameLanguage,
                    NextSessionDate = s.NextSessionDate,
                    NumberOfPlayers = s.Players.Count
                }).ToListAsync())!;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
        return sessions;
    }

    public async Task<ICollection<SessionInfoListViewDto>> GetSessionsFilteredAsync(int page, int pageSize, SessionInfoFilteredRequestDto request)
    {
        throw new NotImplementedException();
    }

    public async Task<ICollection<SessionPlayerInfoDto>> GetSessionPlayerListAsync(string sessionId)
    {
        var players = await _dbContext.SessionPlayers.Where(sp => sp.SessionId.ToString() == sessionId).ToListAsync();
        var usernames = await _userApi.GetSessionPlayersUsernamesAsync(
            players.Where(sp => sp.SessionId.ToString() == sessionId).Select(p => p.AccountId.ToString()).ToList()
            );
        return players.Select(sp => new SessionPlayerInfoDto()
        {
            Username = usernames[sp.AccountId.ToString()],
            Role = sp.Role.ToString()
        }).ToList();
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

    public async Task<SessionInfoSpecificDto?> UpdateSessionInfoAsync(SessionUpdateRequestDto request)
    {
        throw new NotImplementedException();
    }

    public async Task DeleteSessionAsync(string sessionId)
    {
        throw new NotImplementedException();
    }
}