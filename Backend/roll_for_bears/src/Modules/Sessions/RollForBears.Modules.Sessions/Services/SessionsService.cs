using System.Security.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using RollForBears.Modules.Sessions.Contracts.Api;
using RollForBears.Modules.Sessions.Contracts.DTOs;
using RollForBears.Modules.Sessions.Contracts.Enums;
using RollForBears.Modules.Sessions.Database;
using RollForBears.Modules.Sessions.Models;
using RollForBears.Modules.Users.Contracts.Api;

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
    
    public async Task<Guid> CreateSessionAsync(SessionCreateRequestDto request, Guid ownerUuid, CancellationToken cancellationToken)
    {
        var time = DateTime.UtcNow;
        var newSession = new Session()
        {
            Uuid = Guid.NewGuid(),
            Name = request.Name,
            Tagline = request.Tagline,
            IsGeneratingNotes = request.IsGeneratingNotes,
            GameSystem = request.GameSystem,
            GameType = request.GameType,
            MinimalPlayers = request.MinimalPlayers,
            MaximalPlayers = request.MaximalPlayers,
            GameLanguage = request.GameLanguage,
            OwnerUuid = ownerUuid,
            SessionDescription = request.GameDescription,
            NextSessionDate = request.NextSessionDate,
            CreatedAt = time,
            LastUpdatedAt = time
        };
        newSession.Players =
        [
            new SessionPlayer()
            {
                AccountId = ownerUuid,
                ConnectionId = Guid.NewGuid(),
                Role = request.PlayerRole,
                SessionId = newSession.Uuid,
                Session = newSession
            }
        ];
        await _dbContext.AddAsync(newSession, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return newSession.Uuid;
    }

    public async Task<SessionInfoSpecificDto?> GetSpecificSessionInfoAsync(string sessionId, CancellationToken cancellationToken)
    {
        var session = await _dbContext.Sessions.Where(s => s.Uuid.ToString() == sessionId)
            .FirstOrDefaultAsync(cancellationToken);
        if (session == null)
        {
            return null;
        }
        var specificSession = new SessionInfoSpecificDto()
        {
            Uuid = session.Uuid,
            Name = session.Name,
            Tagline = session.Tagline,
            IsGeneratingNotes = session.IsGeneratingNotes,
            GameSystem = session.GameSystem,
            GameType = session.GameType.ToString(),
            OwnerUsername = "",
            MinimalPlayers = session.MinimalPlayers,
            MaximalPlayers = session.MaximalPlayers,
            GameLanguage = session.GameLanguage,
            NextSessionDate = session.NextSessionDate,
            SessionDescription = session.SessionDescription,
            CreatedAt = session.CreatedAt,
            LastUpdatedAt = session.LastUpdatedAt,
            Players = new List<SessionPlayerInfoDto>(),
            JoinRequests = new List<SessionJoinRequestInfoDto>()
        };
        specificSession.OwnerUsername = await _userApi.GetPlayerUsernameAsync(session.OwnerUuid.ToString(), cancellationToken) ?? "";
        specificSession.Players = await GetSessionPlayerListAsync(sessionId, cancellationToken);
        specificSession.JoinRequests = await GetSessionJoinRequestListAsync(sessionId, cancellationToken);
        return specificSession;
    }

    public async Task<ICollection<SessionInfoListViewDto>> GetSessionsAsync(int page, int pageSize, 
        CancellationToken cancellationToken)
    {
        var sessions = 
            await _dbContext.Sessions
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .OrderBy(s => s.CreatedAt)
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
                    NumberOfPlayers = s.Players.Count,
                    LastUpdatedAt = s.LastUpdatedAt
                }).ToListAsync(cancellationToken);
        return sessions;
    }

    public async Task<ICollection<SessionInfoListViewDto>> GetSessionsFilteredAsync(int page, int pageSize, 
        SessionFilteredRequestDto request, CancellationToken cancellationToken)
    {
        var sessions = await _dbContext.Sessions
            .Where(s => request.SessionName == null || s.Name.Contains(request.SessionName))
            .Where(s => request.GameSystem == null || s.GameSystem.Contains(request.GameSystem))
            .Where(s => request.GameType == null || s.GameType.ToString().Equals(request.GameType))
            .Where(s => request.GameLanguage == null || s.GameLanguage.Equals(request.GameLanguage))
            .Where(s => request.MinPlayers == null || s.MinimalPlayers >= request.MinPlayers)
            .Where(s => request.MaxPlayers == null || s.MaximalPlayers <= request.MaxPlayers)
            .Where(s=> request.IsGeneratingNotes == null || request.IsGeneratingNotes == s.IsGeneratingNotes)
            .Where(s => request.SessionBeforeDate == null || request.SessionBeforeDate >= s.NextSessionDate)
            .Where(s => request.SessionAfterDate == null || request.SessionAfterDate <= s.NextSessionDate)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .OrderBy(s => s.CreatedAt)
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
                NumberOfPlayers = s.Players.Count,
                LastUpdatedAt = s.LastUpdatedAt
            }).ToListAsync(cancellationToken);
        return sessions;
    }

    public async Task<ICollection<SessionPlayerInfoDto>> GetSessionPlayerListAsync(string sessionId, 
        CancellationToken cancellationToken)
    {
        var players = await _dbContext.SessionPlayers.Where(sp => sp.SessionId.ToString() == sessionId)
            .ToListAsync(cancellationToken);
        var usernames = await _userApi.GetSessionPlayersUsernamesAsync(
            players.Where(sp => sp.SessionId.ToString() == sessionId)
                             .Select(p => p.AccountId.ToString()).ToList(), cancellationToken);
        return players.Select(sp => new SessionPlayerInfoDto
        {
            Username = usernames[sp.AccountId.ToString()],
            Role = sp.Role.ToString()
        }).ToList();
    }
    
    public async Task<ICollection<SessionJoinRequestInfoDto>> GetSessionJoinRequestListAsync(string sessionId, CancellationToken cancellationToken)
    {
        var players = await _dbContext.JoinRequests.Where(sp => sp.SessionUuid.ToString() == sessionId)
            .ToListAsync(cancellationToken);
        var usernames = await _userApi.GetSessionPlayersUsernamesAsync(
            players.Where(sp => sp.SessionUuid.ToString() == sessionId)
                .Select(p => p.AccountUuid.ToString()).ToList(), cancellationToken);
        return players.Select(sp => new SessionJoinRequestInfoDto()
        {
            Username = usernames[sp.AccountUuid.ToString()],
            Role = sp.PlayerRole.ToString()
        }).ToList();
    }

    public async Task<Guid> AddPlayerToSessionAsync(SessionPlayerAddDto request, Guid userUuid, CancellationToken cancellationToken)
    {
        var session = await _dbContext.Sessions.Include(s => s.JoinRequests)
            .Include(s => s.Players)
            .FirstOrDefaultAsync(s => s.Uuid == request.SessionId, cancellationToken);

        if (session == null)
        {
            throw new BadHttpRequestException("Session doesn't exist");
        }

        if (session.OwnerUuid != userUuid)
        {
            throw new AuthenticationException("You are not authorized to modify this session");
        }
        
        var joinRequest = session.JoinRequests.FirstOrDefault(jr => jr.AccountUuid == request.AccountId);
        
        if (joinRequest == null)
        {
            throw new BadHttpRequestException("No such request found");
        }
        
        if (session.Players.Select(p => p.AccountId).ToList().Contains(request.AccountId))
        {
            throw new BadHttpRequestException("Player is already in this session");
        }

        var sessionPlayer = new SessionPlayer()
        {
            AccountId = request.AccountId,
            SessionId = request.SessionId,
            Role = request.Role,
            ConnectionId = Guid.NewGuid(),
        };
        
        if (request.Role == PlayerRole.Gamemaster)
        {
            foreach (var player in session.Players)
            {
                if (player.Role == PlayerRole.Gamemaster)
                {
                    player.Role = PlayerRole.Player;
                }
            }
        }
        
        await _dbContext.SessionPlayers.AddAsync(sessionPlayer, cancellationToken);
        _dbContext.JoinRequests.Remove(joinRequest);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return request.SessionId;
    }

    public async Task<Guid> ChangePlayerRoleAsync(SessionPlayerAddDto request, Guid userUuid, CancellationToken cancellationToken)
    {
        var session = await _dbContext.Sessions.Include(s => s.JoinRequests)
            .Include(s => s.Players)
            .Where(s => s.Uuid == request.SessionId)
            .FirstOrDefaultAsync(cancellationToken);

        if (session == null)
        {
            throw new BadHttpRequestException("Session doesn't exist");
        }

        if (session.OwnerUuid != userUuid)
        {
            throw new AuthenticationException("You are not authorized to modify this session");
        }

        var player = session.Players.FirstOrDefault(p => p.AccountId == request.AccountId);

        if (player == null)
        {
            throw new BadHttpRequestException("No such player found");
        }
        
        if (request.Role == PlayerRole.Gamemaster)
        {
            foreach (var p in session.Players)
            {
                p.Role = player.AccountId == request.AccountId ? PlayerRole.Gamemaster : PlayerRole.Player;
            }
        } else
        {
            player.Role = request.Role;
        }

        await _dbContext.SaveChangesAsync(cancellationToken);
        return request.SessionId;
    }

    public async Task<Guid> RemovePlayerAsync(SessionPlayerRemoveDto request, Guid userUuid, CancellationToken cancellationToken)
    {
        var session = await _dbContext.Sessions.Include(s => s.Players)
            .FirstOrDefaultAsync(s => s.Uuid == request.SessionId, cancellationToken);

        if (session == null)
        {
            throw new BadHttpRequestException("Session doesn't exist");
        }

        if (session.OwnerUuid != userUuid)
        {
            throw new AuthenticationException("You are not authorized to modify this session");
        }

        var player = session.Players.FirstOrDefault(p => p.AccountId == request.AccountId);

        if (player == null)
        {
            return Guid.Empty;
        }

        if (player.AccountId == session.OwnerUuid)
        {
            throw new BadHttpRequestException("Session owner cannot be removed");
        }
        
        _dbContext.SessionPlayers.Remove(player);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return request.SessionId;
    }

    public async Task<Guid> UpdateSessionInfoAsync(SessionUpdateRequestDto request, Guid userUuid, CancellationToken cancellationToken)
    {
        var session = await _dbContext.Sessions.Include(s => s.Players)
            .FirstOrDefaultAsync(s => s.Uuid == request.Uuid, cancellationToken);

        if (session == null)
        {
            throw new BadHttpRequestException("Session doesn't exist");
        }

        if (session.OwnerUuid != userUuid)
        {
            throw new AuthenticationException("You are not authorized to modify this session");
        }
        
        var currentPlayers = session.Players.Count;
        if (currentPlayers > request.MaximalPlayers)
        {
            throw new BadHttpRequestException("You cannot reduce maximal players bellow current player count");
        }
        
        session.Name = request.Name;
        session.Tagline = request.Tagline;
        session.IsGeneratingNotes = request.IsGeneratingNotes;
        session.GameSystem = request.GameSystem;
        session.GameType = request.GameType;
        session.MinimalPlayers = request.MinimalPlayers;
        session.MaximalPlayers = request.MaximalPlayers;
        session.GameLanguage = request.GameLanguage;
        session.SessionDescription = request.GameDescription;
        session.NextSessionDate = request.NextSessionDate;

        session.LastUpdatedAt = DateTime.UtcNow;
        
        await _dbContext.SaveChangesAsync(cancellationToken);
        return session.Uuid;
    }

    public async Task DeleteSessionAsync(string sessionId, Guid userUuid, CancellationToken cancellationToken)
    {
        var session = await _dbContext.Sessions
            .FirstOrDefaultAsync(s => s.Uuid == Guid.Parse(sessionId), cancellationToken);

        if (session == null)
        {
            throw new BadHttpRequestException("Session doesn't exist");
        }

        if (session.OwnerUuid != userUuid)
        {
            throw new AuthenticationException("You are not authorized to modify this session");
        }
        
        _dbContext.Sessions.Remove(session);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<Guid> AddJoinRequestToSessionAsync(SessionJoinRequestAddDto request, Guid userUuid, CancellationToken cancellationToken)
    {
        var session = await _dbContext.Sessions.Include(s => s.JoinRequests)
            .Where(s => s.Uuid == request.SessionId)
            .Select(s => new
            {
                ownerUuid = s.OwnerUuid,
                JoinRequests = s.JoinRequests.Select(jr => jr.AccountUuid).ToList()
            })
            .FirstOrDefaultAsync(cancellationToken);
        
        if (session == null)
        {
            throw new BadHttpRequestException("Session doesn't exist");
        }
        if (session.ownerUuid == userUuid)
        {
            throw new BadHttpRequestException("You are the owner of this session");
        }
        if (session.JoinRequests.Contains(userUuid))
        {
            throw new BadHttpRequestException("You already requested to join this session");
        }
        
        var joinRequest = new JoinRequest()
        {
            AccountUuid = userUuid,
            ConnectionUuid = Guid.NewGuid(),
            PlayerRole = request.Role,
            SessionUuid = request.SessionId
        };
        
        await _dbContext.JoinRequests.AddAsync(joinRequest, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return joinRequest.SessionUuid;
    }

    public async Task<Guid> RemoveJoinRequestFromSessionAsync(SessionJoinRequestRemoveDto request, Guid userUuid,
        CancellationToken cancellationToken)
    {
        var ownerUuid = await _dbContext.Sessions.Include(s => s.JoinRequests)
            .Where(s => s.Uuid == request.SessionId)
            .Select(s => s.OwnerUuid)
            .FirstOrDefaultAsync(cancellationToken);
        
        if (ownerUuid == Guid.Empty)
        {
            throw new BadHttpRequestException("Session doesn't exist");
        }
        
        if (ownerUuid != userUuid)
        {
            throw new AuthenticationException("You are not authorized to modify this session");
        }
        
        var joinRequest = await _dbContext.JoinRequests.FirstOrDefaultAsync(fr => fr.AccountUuid == request.UserId, cancellationToken);
        
        if (joinRequest == null)
        {
            throw new BadHttpRequestException("No such request found");
        }
        
        _dbContext.JoinRequests.Remove(joinRequest);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return request.SessionId;
    }
}