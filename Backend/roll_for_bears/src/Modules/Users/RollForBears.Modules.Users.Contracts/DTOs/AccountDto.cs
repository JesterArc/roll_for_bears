namespace RollForBears.Modules.Users.Contracts.DTOs;

public sealed record AccountDto(
    Guid Uuid,
    string Email,
    DateOnly CreatedAt,
    DateTime StatusChangedAt
);