namespace DatingApp.Server.Models.Entities;

public class UserModel : IdentityUser<int>
{
    public DateTime DateOfBirth { get; set; }
    public string KnownAs { get; set; }
    public DateTime Created { get; set; } = DateTime.Now;
    public DateTime LastActive { get; set; } = DateTime.Now;
    public string Gender { get; set; }
    public string? Introduction { get; set; }
    public string LookingFor { get; set; } = string.Empty;
    public string Interests { get; set; } = string.Empty;
    public string City { get; set; }
    public string Country { get; set; }

    // Related
    public ICollection<PhotoModel> Photos { get; set; }
    public ICollection<UserLikeModel> LikedByUsers { get; set; }
    public ICollection<UserLikeModel> LikedUsers { get; set; }
    public ICollection<MessageModel> MessagesSent { get; set; }
    public ICollection<MessageModel> MessagesReceived { get; set; }
    public ICollection<AppUserRoleModel> UserRoles { get; set; }
}