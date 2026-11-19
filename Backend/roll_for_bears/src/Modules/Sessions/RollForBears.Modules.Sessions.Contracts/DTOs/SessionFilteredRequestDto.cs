using System.Text.Json.Serialization;
using RollForBears.Modules.Shared_Kernel.JsonConverters;

namespace RollForBears.Modules.Sessions.Contracts.DTOs;

public sealed class SessionFilteredRequestDto
{
    public string? SessionName { get; set; } = null;
    public string? GameSystem { get; set; } = null;
    public string? GameType { get; set; } = null;
    public string? GameLanguage { get; set; } = null;
    public int? MinPlayers { get; set; } = null;
    public int? MaxPlayers { get; set; } = null;
    public bool? IsGeneratingNotes { get; set; } = null;
    [JsonConverter(typeof(CustomJsonDateTimeConverter))]
    public DateTime? SessionBeforeDate { get; set; } = null;
    [JsonConverter(typeof(CustomJsonDateTimeConverter))]
    public DateTime? SessionAfterDate { get; set; } = null;
}