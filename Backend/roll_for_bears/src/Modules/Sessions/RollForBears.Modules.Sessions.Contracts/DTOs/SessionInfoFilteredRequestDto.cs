using RollForBears.Modules.Sessions.Contracts.Enums;

namespace RollForBears.Modules.Sessions.Contracts.DTOs;

public sealed class SessionInfoFilteredRequestDto
{
    public string? SessionName { get; set; }
    public string? GameSystem { get; set; }
    public string? GameType { get; set; }
    public string? GameLanguage { get; set; }
    public int? MinPlayers { get; set; }
    public int? MaxPlayers { get; set; }
    public DateTime? NextSessionDate { get; set; }
}