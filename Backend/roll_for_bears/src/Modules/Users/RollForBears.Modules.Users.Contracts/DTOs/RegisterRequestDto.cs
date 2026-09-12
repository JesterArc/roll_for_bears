using System.ComponentModel.DataAnnotations;

namespace RollForBears.Modules.Users.Contracts.DTOs;

public sealed class RegisterRequestDto
{
    [Required]
    [StringLength(20, MinimumLength = 3)]
    public string Username { get; set; } = null!;
    [Required]
    [MinLength(6)]
    public string Password { get; set; } = null!;
    [Required]
    [EmailAddress]
    public string Email { get; set; } = null!;
}