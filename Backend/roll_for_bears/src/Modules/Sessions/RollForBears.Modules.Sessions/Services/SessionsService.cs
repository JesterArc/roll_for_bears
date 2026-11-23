using Microsoft.EntityFrameworkCore;
using RollForBears.Modules.Sessions.Contracts.Api;
using RollForBears.Modules.Sessions.Contracts.DTOs;
using RollForBears.Modules.Sessions.Contracts.Enums;
using RollForBears.Modules.Sessions.Database;
using RollForBears.Modules.Sessions.Models;
using RollForBears.Modules.Shared_Kernel.Exceptions;
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
        var newSession = new Session
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
            new SessionPlayer
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

    public async Task<SessionInfoSpecificDto?> GetSpecificSessionInfoAsync(Guid sessionUuid, CancellationToken cancellationToken)
    {
        var session = await _dbContext.Sessions.Where(s => s.Uuid == sessionUuid)
            .AsNoTracking()
            .FirstOrDefaultAsync(cancellationToken);
        if (session == null)
        {
            throw new NotFoundException("Session not found");
        }
        var specificSession = new SessionInfoSpecificDto
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
        // TODO: change method of getting session players so that we can get owner username without separate call
        specificSession.OwnerUsername = await _userApi.GetPlayerUsernameAsync(session.OwnerUuid, cancellationToken) ?? "";
        specificSession.Players = await GetSessionPlayerListAsync(sessionUuid, cancellationToken);
        specificSession.JoinRequests = await GetSessionJoinRequestListAsync(sessionUuid, cancellationToken);
        return specificSession;
    }

    public async Task<ICollection<SessionInfoListViewDto>> GetSessionsAsync(int page, int pageSize, 
        CancellationToken cancellationToken)
    {
        var sessions = 
            await _dbContext.Sessions
                .OrderBy(s => s.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(s => new SessionInfoListViewDto
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
        var sessions = await _dbContext.Sessions.AsNoTracking()
            .Where(s => request.SessionName == null || s.Name.Contains(request.SessionName))
            .Where(s => request.GameSystem == null || s.GameSystem.Contains(request.GameSystem))
            .Where(s => request.GameType == null || s.GameType.ToString().Equals(request.GameType))
            .Where(s => request.GameLanguage == null || s.GameLanguage.Equals(request.GameLanguage))
            .Where(s => request.MinPlayers == null || s.MinimalPlayers >= request.MinPlayers)
            .Where(s => request.MaxPlayers == null || s.MaximalPlayers <= request.MaxPlayers)
            .Where(s=> request.IsGeneratingNotes == null || request.IsGeneratingNotes == s.IsGeneratingNotes)
            .Where(s => request.SessionBeforeDate == null || request.SessionBeforeDate >= s.NextSessionDate)
            .Where(s => request.SessionAfterDate == null || request.SessionAfterDate <= s.NextSessionDate)
            .OrderBy(s => s.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(s => new SessionInfoListViewDto
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

    public async Task<ICollection<SessionPlayerInfoDto>> GetSessionPlayerListAsync(Guid sessionUuid, 
        CancellationToken cancellationToken)
    {
        var players = await _dbContext.SessionPlayers.AsNoTracking()
            .Where(sp => sp.SessionId == sessionUuid)
            .ToListAsync(cancellationToken);
        var usernames = await _userApi.GetUsernamesAsync(
            players.Select(p => p.AccountId).ToList(), cancellationToken);
        return players.Select(sp => new SessionPlayerInfoDto
        {
            Username = usernames[sp.AccountId],
            Role = sp.Role
        }).ToList();
    }
    
    public async Task<ICollection<SessionJoinRequestInfoDto>> GetSessionJoinRequestListAsync(Guid sessionUuid, CancellationToken cancellationToken)
    {
        var players = await _dbContext.JoinRequests.Where(sp => sp.SessionUuid == sessionUuid)
            .ToListAsync(cancellationToken);
        var usernames = await _userApi.GetUsernamesAsync(
            players.Select(p => p.AccountUuid).ToList(), cancellationToken);
        return players.Select(sp => new SessionJoinRequestInfoDto()
        {
            Username = usernames[sp.AccountUuid],
            Role = sp.PlayerRole.ToString()
        }).ToList();
    }

    public async Task<Guid> AddPlayerToSessionAsync(Guid sessionUuid, SessionPlayerDto request, Guid userUuid, CancellationToken cancellationToken)
    {
        var session = await _dbContext.Sessions.Include(s => s.JoinRequests)
            .Include(s => s.Players)
            .FirstOrDefaultAsync(s => s.Uuid == sessionUuid, cancellationToken);

        if (session == null)
        {
            throw new NotFoundException("Session not found");
        }

        if (session.OwnerUuid != userUuid)
        {
            throw new AccessForbiddenException("You are not authorized to modify this session");
        }
        
        var joinRequest = session.JoinRequests.FirstOrDefault(jr => jr.AccountUuid == request.AccountUuid);
        
        if (joinRequest == null)
        {
            throw new NotFoundException("Request not found");
        }
        
        if (session.Players.Select(p => p.AccountId).ToList().Contains(request.AccountUuid))
        {
            throw new BadRequestException("Player is already in this session");
        }

        var sessionPlayer = new SessionPlayer
        {
            AccountId = request.AccountUuid,
            SessionId = sessionUuid,
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
        return sessionUuid;
    }

    public async Task<Guid> ChangePlayerRoleAsync(Guid sessionUuid, SessionPlayerDto request, Guid userUuid, CancellationToken cancellationToken)
    {
        var session = await _dbContext.Sessions.Include(s => s.JoinRequests)
            .Include(s => s.Players)
            .Where(s => s.Uuid == sessionUuid)
            .FirstOrDefaultAsync(cancellationToken);

        if (session == null)
        {
            throw new NotFoundException("Session not found");
        }

        if (session.OwnerUuid != userUuid)
        {
            throw new AccessForbiddenException("You are not authorized to modify this session");
        }

        var player = session.Players.FirstOrDefault(p => p.AccountId == request.AccountUuid);

        if (player == null)
        {
            throw new NotFoundException("Player not found");
        }
        
        if (request.Role == PlayerRole.Gamemaster)
        {
            foreach (var p in session.Players)
            {
                p.Role = player.AccountId == request.AccountUuid ? PlayerRole.Gamemaster : PlayerRole.Player;
            }
        } else
        {
            player.Role = request.Role;
        }

        await _dbContext.SaveChangesAsync(cancellationToken);
        return sessionUuid;
    }

    public async Task<Guid> RemovePlayerAsync(SessionPlayerRemoveDto request, Guid userUuid, CancellationToken cancellationToken)
    {
        var session = await _dbContext.Sessions.Include(s => s.Players)
            .FirstOrDefaultAsync(s => s.Uuid == request.SessionId, cancellationToken);

        if (session == null)
        {
            throw new NotFoundException("Session not found");
        }

        if (session.OwnerUuid != userUuid)
        {
            throw new AccessForbiddenException("You are not authorized to modify this session");
        }

        var player = session.Players.FirstOrDefault(p => p.AccountId == request.AccountId);

        if (player == null)
        {
            return Guid.Empty;
        }

        if (player.AccountId == session.OwnerUuid)
        {
            throw new BadRequestException("Session owner cannot be removed");
        }
        
        _dbContext.SessionPlayers.Remove(player);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return request.SessionId;
    }

    public async Task<Guid> UpdateSessionInfoAsync(Guid sessionUuid, SessionUpdateRequestDto request, Guid userUuid, CancellationToken cancellationToken)
    {
        var session = await _dbContext.Sessions.Include(s => s.Players)
            .FirstOrDefaultAsync(s => s.Uuid == sessionUuid, cancellationToken);

        if (session == null)
        {
            throw new NotFoundException("Session not found");
        }

        if (session.OwnerUuid != userUuid)
        {
            throw new AccessForbiddenException("You are not authorized to modify this session");
        }
        
        var currentPlayers = session.Players.Count;
        
        if (currentPlayers > request.MaximalPlayers)
        {
            throw new BadRequestException("You cannot reduce maximal players bellow current player count");
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

    public async Task DeleteSessionAsync(Guid sessionUuid, Guid userUuid, CancellationToken cancellationToken)
    {
        var session = await _dbContext.Sessions
            .FirstOrDefaultAsync(s => s.Uuid == sessionUuid, cancellationToken);

        if (session == null)
        {
            throw new NotFoundException("Session not found");
        }

        if (session.OwnerUuid != userUuid)
        {
            throw new AccessForbiddenException("You are not authorized to modify this session");
        }
        
        _dbContext.Sessions.Remove(session);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<Guid> AddJoinRequestToSessionAsync(Guid sessionUuid, SessionJoinRequestRoleDto request, Guid userUuid, CancellationToken cancellationToken)
    {
        var session = await _dbContext.Sessions.Include(s => s.JoinRequests)
            .Include(s => s.Players)
            .Where(s => s.Uuid == sessionUuid)
            .Select(s => new
            {
                ownerUuid = s.OwnerUuid,
                JoinRequests = s.JoinRequests.Select(jr => jr.AccountUuid).ToList(),
                Players = s.Players.Select(p => p.AccountId).ToList()
            })
            .FirstOrDefaultAsync(cancellationToken);
        
        if (session == null)
        {
            throw new NotFoundException("Session not found");
        }

        if (session.Players.Contains(userUuid))
        {
            throw new BadRequestException("You already have joined this session");
        }
        
        if (session.JoinRequests.Contains(userUuid))
        {
            throw new BadRequestException("You already have requested to join this session");
        }
        
        var joinRequest = new JoinRequest
        {
            AccountUuid = userUuid,
            ConnectionUuid = Guid.NewGuid(),
            PlayerRole = request.Role,
            SessionUuid = sessionUuid
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
            throw new NotFoundException("Session not found");
        }
        
        if (ownerUuid != userUuid)
        {
            throw new AccessForbiddenException("You are not authorized to modify this session");
        }
        
        var joinRequest = await _dbContext.JoinRequests.FirstOrDefaultAsync(fr => fr.AccountUuid == request.UserId, cancellationToken);
        
        if (joinRequest == null)
        {
            throw new NotFoundException("Request not found");
        }
        
        _dbContext.JoinRequests.Remove(joinRequest);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return request.SessionId;
    }
}