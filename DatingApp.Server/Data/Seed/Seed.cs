namespace DatingApp.Server.Data.Seed;

public class Seed
{
    public static async Task SeedUsers(UserManager<UserModel> userManager, RoleManager<AppRoleModel> roleManager)
    {
        if (await userManager.Users.AnyAsync()) return;
        var userData = await File.ReadAllTextAsync("Data/UserSeedData.json");
        var users = JsonSerializer.Deserialize<List<UserModel>>(userData);

        if (users == null) return;

        var roles = new List<AppRoleModel>()
        {
            new AppRoleModel{Name = "Admin"},
            new AppRoleModel{Name = "Moderator"},
            new AppRoleModel{Name = "Member"},
        };

        foreach (var role in roles) await roleManager.CreateAsync(role);

        foreach (var user in users)
        {
            user.UserName = user.UserName.ToLower();
            await userManager.CreateAsync(user, "Welcome1");
            await userManager.AddToRoleAsync(user, "Member");
        }

        var admin = new UserModel
        {
            UserName = "admin",
            Gender = "male",
            DateOfBirth = DateTime.Now,
            KnownAs = string.Empty,
            Created = DateTime.Now,
            LastActive = DateTime.Now,
            Introduction = string.Empty,
            LookingFor = string.Empty,
            Interests = string.Empty,
            City = string.Empty,
            Country = string.Empty
        };

        await userManager.CreateAsync(admin, "Welcome1");
        await userManager.AddToRolesAsync(admin, new [] {"Admin", "Moderator"});
    }
}
