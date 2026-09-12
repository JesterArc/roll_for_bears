namespace RollForBears.Modules.Users.Contracts.DTOs;

public sealed record LoginResultDto
(
    Guid Uuid,
    string Username,
    string AccessToken,
    string RefreshToken
);