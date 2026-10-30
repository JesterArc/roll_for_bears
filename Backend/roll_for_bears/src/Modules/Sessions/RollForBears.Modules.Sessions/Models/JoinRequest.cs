using RollForBears.Modules.Sessions.Contracts.Enums;

namespace RollForBears.Modules.Sessions.Models;

public partial class JoinRequest
{
    public required Guid ConnectionUuid { get; set; }
    public required Guid SessionUuid { get; set; }
    public required Guid AccountUuid { get; set; }
    public required PlayerRole PlayerRole { get; set; }

    public virtual Session Session { get; set; } = null!;
}
