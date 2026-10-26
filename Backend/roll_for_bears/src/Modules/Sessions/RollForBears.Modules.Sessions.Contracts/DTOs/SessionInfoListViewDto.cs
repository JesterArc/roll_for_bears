namespace RollForBears.Modules.Sessions.Contracts.DTOs;

public sealed class SessionInfoListViewDto
{
    public Guid Uuid { get; set; }
    public required string Name { get; set; } = null!;
    public string? Tagline {get; set; }
    public required string GameSystem { get; set; } = null!;
    public required string GameType { get; set; }
    public required int NumberOfPlayers { get; set; }
    public required int MaximalPlayers { get; set; }
    public required string GameLanguage { get; set; } = null!;
    public DateTime? NextSessionDate { get; set; }
}