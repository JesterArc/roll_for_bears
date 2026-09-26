using RollForBears.Modules.Sessions.Contracts.Enums;
using RollForBears.Modules.Users.Models;

namespace RollForBears.Modules.Sessions.Models;

public partial class SessionPlayer
{
    public Guid ConnectionId { get; set; }
    public Guid AccountId { get; set; }
    public Guid SessionId { get; set; }
    public PlayerRole Role { get; set; }
    
    public virtual Session Session { get; set; } = null!;
    public virtual Account Account { get; set; } = null!;
}