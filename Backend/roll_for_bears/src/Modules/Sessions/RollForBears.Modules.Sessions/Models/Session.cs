
using RollForBears.Modules.Sessions.Contracts.Enums;
using RollForBears.Modules.Users.Models;

namespace RollForBears.Modules.Sessions.Models;

public partial class Session
{
    public Guid Uuid { get; set; }
    public required string Name { get; set; } = null!;
    public string? Tagline {get; set; }
    public bool IsGeneratingNotes {get; set; } 
    //TODO: replace string with actual data type for GameSystem
    public required string GameSystem { get; set; } = null!;
    public required GameType GameType { get; set; }
    public required int MinimalPlayers { get; set; }
    public required int MaximalPlayers { get; set; }
    //TODO: replace string with actual data type for GameLanguage
    public required string GameLanguage { get; set; } = null!;
    public Guid OwnerUuid { get; set; }
    public required string SessionDescription { get; set; } = null!;
    public DateTime? NextSessionDate { get; set; }
    
    public virtual ICollection<SessionPlayer> Players { get; set; } = new List<SessionPlayer>();
    public virtual Account Owner { get; set; } = null!;
}