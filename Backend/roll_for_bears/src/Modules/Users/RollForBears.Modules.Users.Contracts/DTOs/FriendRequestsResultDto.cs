namespace RollForBears.Modules.Users.Contracts.DTOs;

public sealed class FriendRequestsResultDto
{
    public List<FriendDto> IncomingFriendRequests = null!;
    public List<FriendDto> OutgoingFriendRequests = null!;
}