namespace RollForBears.Modules.Sessions.Contracts.DTOs;

public sealed class SessionFilteredRequestDto
{
    public string? SessionName { get; set; }
    public string? GameSystem { get; set; }
    public string? GameType { get; set; }
    public string? GameLanguage { get; set; }
    public int? MinPlayers { get; set; }
    public int? MaxPlayers { get; set; }
    public bool? IsGeneratingNotes { get; set; }
    public DateTime? SessionBeforeDate { get; set; }
    public DateTime? SessionAfterDate { get; set; }
}