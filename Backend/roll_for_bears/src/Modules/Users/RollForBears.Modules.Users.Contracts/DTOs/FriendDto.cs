namespace RollForBears.Modules.Users.Contracts.DTOs;

public sealed record FriendDto
{
    public string Username { get; set; }
    public bool FriendshipMutual { get; set; } = false;
}