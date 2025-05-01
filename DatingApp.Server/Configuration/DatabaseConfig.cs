namespace DatingApp.Server.Configuration;

public static class DatabaseConfig
{
    public static async Task ConfigureDatabase(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var services = scope.ServiceProvider;

        try
        {
            var db = services.GetRequiredService<DataContext>();
            var userManager = services.GetRequiredService<UserManager<UserModel>>();
            var roleManager = services.GetRequiredService<RoleManager<AppRoleModel>>();
            await db.Database.MigrateAsync();
            await Seed.SeedUsers(userManager, roleManager);
        }
        catch (Exception ex)
        {
            var logger = services.GetRequiredService<ILogger<Program>>();
            logger.LogError(ex, "An error occurred during migration");
        }
    }
}
