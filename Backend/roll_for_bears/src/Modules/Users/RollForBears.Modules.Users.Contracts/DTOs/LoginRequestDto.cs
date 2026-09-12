using System.ComponentModel.DataAnnotations;

namespace RollForBears.Modules.Users.Contracts.DTOs;

public sealed class LoginRequestDto
{
    [Required]
    public string Username { get; set; } = null!;

    [Required] 
    public string Password { get; set; } = null!;
    
}