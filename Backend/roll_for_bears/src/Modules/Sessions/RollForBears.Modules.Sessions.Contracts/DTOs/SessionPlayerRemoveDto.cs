using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using RollForBears.Modules.Shared_Kernel.JsonConverters;

namespace RollForBears.Modules.Sessions.Contracts.DTOs;

public sealed class SessionPlayerRemoveDto
{
    [Required]
    [JsonConverter(typeof(CustomJsonGuidConverter))]
    public Guid AccountId { get; set; }
    [Required]
    [JsonConverter(typeof(CustomJsonGuidConverter))]
    public Guid SessionId { get; set; }
}