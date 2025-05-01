namespace DatingApp.Server.Configuration;

public static class IdentityServices
{
    public static IServiceCollection AddIdentityServices(this IServiceCollection services, IConfiguration config)
    {
        services.AddIdentityCore<UserModel>(options =>
        {
            options.Password.RequireNonAlphanumeric = false;
        })
        .AddRoles<AppRoleModel>()
        .AddUserManager<UserManager<UserModel>>()
        .AddRoleManager<RoleManager<AppRoleModel>>()
        .AddSignInManager<SignInManager<UserModel>>()
        .AddRoleValidator<RoleValidator<AppRoleModel>>()
        .AddEntityFrameworkStores<DataContext>();

        // authentication
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
        {
            // client send token as auth header
            options.TokenValidationParameters = new TokenValidationParameters()
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config.GetValue<string>("TokenKey"))),
                ValidateIssuer = false,
                ValidateAudience = false
            };

            // client sends token as url query string
            options.Events = new JwtBearerEvents()
            {
                OnMessageReceived = context =>
                {
                    var accessToken = context.Request.Query["access_token"];
                    var path = context.HttpContext.Request.Path;
                    if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/hubs")) context.Token = accessToken;
                    return Task.CompletedTask;
                }
            };
        });

        // authorisation
        services.AddAuthorization(opt => opt.AddPolicy("RequireAdminRole", policy => policy.RequireRole("Admin")));

        return services;
    }
}
