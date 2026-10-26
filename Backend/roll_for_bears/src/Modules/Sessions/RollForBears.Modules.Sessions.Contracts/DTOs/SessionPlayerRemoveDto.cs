namespace RollForBears.Modules.Sessions.Contracts.DTOs;

public sealed class SessionPlayerRemoveDto
{
    public Guid AccountId { get; set; }
    public Guid SessionId { get; set; }
}