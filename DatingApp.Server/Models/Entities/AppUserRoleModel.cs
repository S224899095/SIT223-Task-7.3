namespace DatingApp.Server.Models.Entities;

public class AppUserRoleModel : IdentityUserRole<int>
{
    public UserModel User { get; set; }
    public AppRoleModel AppRole { get; set; }
}
