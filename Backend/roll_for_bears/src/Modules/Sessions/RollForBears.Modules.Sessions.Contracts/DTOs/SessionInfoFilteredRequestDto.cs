using RollForBears.Modules.Sessions.Contracts.Enums;

namespace RollForBears.Modules.Sessions.Contracts.DTOs;

public sealed class SessionInfoFilteredRequestDto
{
    public string? SessionName { get; set; }
    //TODO: replace string with actual data type for GameSystem
    public string? GameSystem { get; set; }
    public GameType? GameType { get; set; }
    //TODO: replace string with actual data type for GameLanguage
    public string? GameLanguage { get; set; }
    public int? MinPlayers { get; set; }
    public int? MaxPlayers { get; set; }
    public DateTime? NextSessionDate { get; set; }
}