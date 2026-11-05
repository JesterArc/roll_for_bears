namespace RollForBears.Modules.Users.Models;

public partial class Friend
{
    public Guid FriendshipUuid { get; set; }
    public Guid UserUuid { get; set; }
    public Guid FriendUuid { get; set; }
    public bool FriendshipMutual { get; set; } = false;
    
    public virtual Account User { get; set; } = null!;
    public virtual Account UsersFriend { get; set; } = null!;
}