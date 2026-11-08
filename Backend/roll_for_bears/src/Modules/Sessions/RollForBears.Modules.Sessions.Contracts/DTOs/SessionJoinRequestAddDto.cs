using RollForBears.Modules.Sessions.Contracts.Enums;

namespace RollForBears.Modules.Sessions.Contracts.DTOs;

public class SessionJoinRequestAddDto
{
    public Guid SessionId { get; set; }
    public PlayerRole Role { get; set; }
}
