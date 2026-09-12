

namespace RollForBears.Modules.Users.Models;

public partial class Account
{
    public Guid Uuid { get; set; }

    public string HashWord { get; set; } = null!;

    public DateOnly CreatedAt { get; set; }

    public string Email { get; set; } = null!;

    public DateTime StatusChangedAt { get; set; }

    public string? Username { get; set; }

    public virtual ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
    public AccountStatus Status { get; set; }
}
