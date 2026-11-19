using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using RollForBears.Modules.Shared_Kernel.JsonConverters;

namespace RollForBears.Modules.Sessions.Contracts.DTOs;

public sealed record SessionInfoSpecificDto
{
    [Required]
    [JsonConverter(typeof(CustomJsonGuidConverter))]
    public Guid Uuid { get; set; }
    [Required]
    public string Name { get; set; } = null!;
    public string? Tagline {get; set; }
    [Required]
    public bool IsGeneratingNotes {get; set; } 
    [Required]
    public string GameSystem { get; set; } = null!;
    [Required]
    public string GameType { get; set; } = null!;
    [Required]
    public int MinimalPlayers { get; set; }
    [Required]
    public int MaximalPlayers { get; set; }
    [Required] 
    public string GameLanguage { get; set; } = null!;
    [Required] 
    public string OwnerUsername { get; set; } = null!;
    [Required] 
    public string SessionDescription { get; set; } = null!;
    [JsonConverter(typeof(CustomJsonDateTimeConverter))]
    public DateTime? NextSessionDate { get; set; }
    [Required]
    [JsonConverter(typeof(CustomJsonDateTimeConverter))]
    public DateTime CreatedAt { get; set; }
    [Required]
    [JsonConverter(typeof(CustomJsonDateTimeConverter))]
    public DateTime LastUpdatedAt { get; set; }
    public ICollection<SessionPlayerInfoDto> Players { get; set; } = new List<SessionPlayerInfoDto>();
    public ICollection<SessionJoinRequestInfoDto> JoinRequests { get; set; } = new List<SessionJoinRequestInfoDto>();
}