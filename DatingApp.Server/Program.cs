using Serilog;
using Serilog.Events;

namespace DatingApp.Server;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.Services.AddServices(builder.Configuration);

        // Set up Serilog
        var env = builder.Environment;
        var logPath = builder.Configuration.GetValue<string>("LogFilePath")!; // fallback if not set
        var fullLogPath = Path.Combine(env.ContentRootPath, logPath);

        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
            .Enrich.FromLogContext()
            .Enrich.WithProperty("service", "datingapp") // Replace as needed
            .Enrich.WithProperty("env", env.EnvironmentName)
            .WriteTo.Console()
            .WriteTo.File(fullLogPath, rollingInterval: RollingInterval.Day)
            .CreateLogger();

        builder.Host.UseSerilog();

        // configure http/https and listening ports
        var isDevelopment = builder.Environment.IsDevelopment();
        builder.WebHost.ConfigureKestrel(options =>
        {
            if (isDevelopment)
            {
                // Local dev: HTTPS on port 7014
                options.ListenAnyIP(7014, listenOptions =>
                {
                    listenOptions.UseHttps("localhost.pfx", "password");
                });
            }
        });

        var app = builder.Build();
        await app.Config(builder);
        await app.RunAsync();
    }
}
