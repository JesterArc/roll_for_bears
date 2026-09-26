using RollForBears.Modules.Sessions.Contracts.Enums;

namespace RollForBears.Modules.Sessions.Contracts.DTOs;

public sealed class SessionPlayerInfoDto
{
    public required string Username { get; set; } = null!;
    public PlayerRole Role { get; set; }
}