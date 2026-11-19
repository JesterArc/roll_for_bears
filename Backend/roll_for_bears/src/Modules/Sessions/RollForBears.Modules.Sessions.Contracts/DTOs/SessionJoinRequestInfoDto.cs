using System.ComponentModel.DataAnnotations;

namespace RollForBears.Modules.Sessions.Contracts.DTOs;

public sealed class SessionJoinRequestInfoDto
{
    [Required]
    public string Username { get; set; } = null!;
    [Required]
    public string Role { get; set; } = null!;
}
