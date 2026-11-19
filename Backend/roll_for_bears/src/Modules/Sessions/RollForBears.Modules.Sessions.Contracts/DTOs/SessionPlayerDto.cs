using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using RollForBears.Modules.Sessions.Contracts.Enums;
using RollForBears.Modules.Shared_Kernel.JsonConverters;

namespace RollForBears.Modules.Sessions.Contracts.DTOs;

public sealed class SessionPlayerDto
{
    [Required]
    [JsonConverter(typeof(CustomJsonGuidConverter))]
    public Guid AccountUuid { get; set; }
    [Required]
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public PlayerRole Role { get; set; }
}