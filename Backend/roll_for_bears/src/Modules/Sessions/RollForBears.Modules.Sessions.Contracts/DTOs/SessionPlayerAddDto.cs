using RollForBears.Modules.Sessions.Contracts.Enums;

namespace RollForBears.Modules.Sessions.Contracts.DTOs;

public sealed class SessionPlayerAddDto
{
    public Guid AccountId { get; set; }
    public Guid SessionId { get; set; }
    public PlayerRole Role { get; set; }
}