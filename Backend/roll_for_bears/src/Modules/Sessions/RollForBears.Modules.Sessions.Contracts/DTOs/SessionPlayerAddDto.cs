using RollForBears.Modules.Sessions.Contracts.Enums;

namespace RollForBears.Modules.Sessions.Contracts.DTOs;

public sealed class SessionPlayerAddDto
{
    public Guid SessionId { get; set; }
    public Guid UserId { get; set; }
    public PlayerRole Role { get; set; }
}