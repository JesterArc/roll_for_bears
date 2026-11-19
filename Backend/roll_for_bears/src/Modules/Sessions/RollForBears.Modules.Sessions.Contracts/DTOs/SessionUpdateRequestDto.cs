using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using RollForBears.Modules.Sessions.Contracts.Enums;
using RollForBears.Modules.Shared_Kernel.JsonConverters;

namespace RollForBears.Modules.Sessions.Contracts.DTOs;

public sealed class SessionUpdateRequestDto
{
    [Required]
    public string Name { get; set; } = null!;
    public string? Tagline {get; set; }
    [Required]
    public bool IsGeneratingNotes {get; set; } 
    [Required]
    public string GameSystem { get; set; } = null!;
    [JsonConverter(typeof(JsonStringEnumConverter))]
    [Required]
    public GameType GameType { get; set; }
    [Required]
    public int MinimalPlayers { get; set; }
    [Required]
    public int MaximalPlayers { get; set; }
    [Required]
    public string GameLanguage { get; set; } = null!;
    [Required]
    public string GameDescription { get; set; } = null!;
    [JsonConverter(typeof(CustomJsonDateTimeConverter))]
    public DateTime? NextSessionDate { get; set; }
}