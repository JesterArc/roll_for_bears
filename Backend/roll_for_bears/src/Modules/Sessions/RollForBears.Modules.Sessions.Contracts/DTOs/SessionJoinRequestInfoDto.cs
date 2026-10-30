namespace RollForBears.Modules.Sessions.Contracts.DTOs;

public sealed class SessionJoinRequestInfoDto
{
    public required string Username { get; set; } = null!;
    public required string Role { get; set; }
}
