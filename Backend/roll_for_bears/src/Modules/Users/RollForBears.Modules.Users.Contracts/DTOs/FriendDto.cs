namespace RollForBears.Modules.Users.Contracts.DTOs;

public sealed record FriendDto
{
    public Guid FriendUuid { get; set; }
    public bool FriendshipMutual { get; set; } = false;
}