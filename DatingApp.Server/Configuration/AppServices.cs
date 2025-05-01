using Serilog;

namespace DatingApp.Server.Configuration;

public static class Services
{
    public static IServiceCollection AddAppServices(this IServiceCollection services)
    {
        services.AddAutoMapper(typeof(AutoMapperProfiles).Assembly);
        services.AddScoped<IPhotoService, PhotoService>();
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<LogUserActivity>();
        services.AddSingleton<PresenceTracker>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        
        return services;
    }
}
