using System.ComponentModel.DataAnnotations;
using RollForBears.Modules.Sessions.Contracts.Enums;

namespace RollForBears.Modules.Sessions.Models;

public partial class Session
{
    public required Guid Uuid { get; set; }
    [MaxLength(100)]
    public required string Name { get; set; } = null!;
    [MaxLength(50)]
    public string? Tagline {get; set; }
    public required bool IsGeneratingNotes {get; set; } 
    //TODO: replace string with data type for GameSystem
    public required string GameSystem { get; set; } = null!;
    public required GameType GameType { get; set; }
    public required int MinimalPlayers { get; set; }
    public required int MaximalPlayers { get; set; }
    [MaxLength(100)]
    public required string GameLanguage { get; set; } = null!;
    public required Guid OwnerUuid { get; set; }
    [MaxLength(1024)]
    public required string SessionDescription { get; set; } = null!;
    public DateTime? NextSessionDate { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime LastUpdatedAt { get; set; }
    
    public virtual ICollection<SessionPlayer> Players { get; set; } = new List<SessionPlayer>();
    public virtual ICollection<JoinRequest> JoinRequests { get; set; } = new List<JoinRequest>();
}