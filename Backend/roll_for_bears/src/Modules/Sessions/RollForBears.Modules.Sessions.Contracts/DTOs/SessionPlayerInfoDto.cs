using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace RollForBears.Modules.Sessions.Contracts.DTOs;

public sealed class SessionPlayerInfoDto
{
    [Required]
    public string Username { get; set; } = null!;
    [Required]
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public string Role { get; set; } = null!;
}