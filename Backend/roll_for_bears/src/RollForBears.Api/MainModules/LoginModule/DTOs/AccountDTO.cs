namespace roll_for_bears.MainModules.LoginModule.DTOs;

public class AccountDto
{
    public DateOnly CreatedAt { get; set; }

    public string Email { get; set; } = null!;
}