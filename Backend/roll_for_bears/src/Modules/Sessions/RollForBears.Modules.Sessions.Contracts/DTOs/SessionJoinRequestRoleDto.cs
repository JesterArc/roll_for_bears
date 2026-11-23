using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using RollForBears.Modules.Sessions.Contracts.Enums;

namespace RollForBears.Modules.Sessions.Contracts.DTOs;

public sealed class SessionJoinRequestRoleDto
{
    [Required]
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public PlayerRole Role { get; set; }
}
