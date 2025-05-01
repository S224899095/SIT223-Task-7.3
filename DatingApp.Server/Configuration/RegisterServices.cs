using Serilog;

namespace DatingApp.Server.Configuration;

public static class RegisterServices
{
    public static void AddServices(this IServiceCollection services, IConfiguration config)
    {
        services.Configure<CloudinarySettings>(config.GetSection("CloudinarySettings"));
        services.AddControllers();
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();
        services.AddAppServices();
        services.AddIdentityServices(config);
        services.AddSignalR();
        services.AddDbContext<DataContext>(opt => opt.UseSqlite(config.GetConnectionString("DefaultConnection")!));
    }
}
