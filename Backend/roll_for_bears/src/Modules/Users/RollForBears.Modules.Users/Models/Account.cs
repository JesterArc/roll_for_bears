using System;
using System.Collections.Generic;

namespace RollForBears.Modules.Users.Models;

public partial class Account
{
    public Guid Uuid { get; set; }

    public string Password { get; set; } = null!;

    public DateOnly CreatedAt { get; set; }

    public string Email { get; set; } = null!;

    public DateTime StatusChangedAt { get; set; }
}
