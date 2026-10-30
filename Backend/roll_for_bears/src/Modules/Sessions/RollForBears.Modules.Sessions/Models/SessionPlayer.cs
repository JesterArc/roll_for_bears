using RollForBears.Modules.Sessions.Contracts.Enums;

namespace RollForBears.Modules.Sessions.Models;

public partial class SessionPlayer
{
    public required Guid ConnectionId { get; set; }
    public required Guid AccountId { get; set; }
    public required Guid SessionId { get; set; }
    public required PlayerRole Role { get; set; }
    
    public virtual Session Session { get; set; } = null!;
}