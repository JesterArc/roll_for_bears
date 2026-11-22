using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using RollForBears.Modules.Shared_Kernel.JsonConverters;

namespace RollForBears.Modules.Users.Contracts.DTOs;

public sealed class FriendRequestDto
{
    [Required]
    [JsonConverter(typeof(CustomJsonGuidConverter))]
    public Guid FriendUuid { get; set; }
}