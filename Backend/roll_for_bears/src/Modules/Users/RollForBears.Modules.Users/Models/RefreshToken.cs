using System;
using System.Collections.Generic;

namespace RollForBears.Modules.Users.Models;

public partial class RefreshToken
{
    public Guid Uuid { get; set; }

    public Guid AccountId { get; set; }

    public string TokenHash { get; set; } = null!;

    public Guid FamilyId { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime ExpiresAt { get; set; }

    public DateTime? RevokedAt { get; set; }

    public Guid? ReplacedByTokenId { get; set; }

    public virtual Account Account { get; set; } = null!;

    public virtual ICollection<RefreshToken> InverseReplacedByToken { get; set; } = new List<RefreshToken>();

    public virtual RefreshToken? ReplacedByToken { get; set; }
}
