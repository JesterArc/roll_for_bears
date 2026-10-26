using RollForBears.Modules.Sessions.Contracts.Enums;

namespace RollForBears.Modules.Sessions.Contracts.DTOs;

public sealed class SessionUpdateRequestDto
{
    public Guid Uuid { get; set; }
    public required string Name { get; set; } = null!;
    public string? Tagline {get; set; }
    public bool IsGeneratingNotes {get; set; } 
    public required string GameSystem { get; set; } = null!;
    public required GameType GameType { get; set; }
    public required int MinimalPlayers { get; set; }
    public required int MaximalPlayers { get; set; }
    public required string GameLanguage { get; set; } = null!;
    public required string GameDescription { get; set; } = null!;
    public DateTime? NextSessionDate { get; set; }
}