namespace RollForBears.Modules.Sessions.Contracts.DTOs;

public sealed class SessionJoinRequestRemoveDto
{
    public Guid SessionId { get; set; }
    public Guid UserId { get; set; }
}