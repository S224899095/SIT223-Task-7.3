namespace DatingApp.Server.Models.Entities;

public class AppRoleModel : IdentityRole<int>
{
    public ICollection<AppUserRoleModel> UserRoles { get; set; }
}
