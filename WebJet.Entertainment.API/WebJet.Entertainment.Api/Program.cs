using Serilog;
using Serilog.Sinks.SystemConsole.Themes;


namespace WebJet.Entertainment.Api;


public class Program
{
    public static void Main(string[] args)
    {
        try
        {
            Log.Logger = new LoggerConfiguration()
                        .WriteTo.Console(theme: AnsiConsoleTheme.Code)
                        .Enrich.FromLogContext()
                        .MinimumLevel.Information()
                        .CreateBootstrapLogger();

            Log.Logger.Information("Setting up Web Application....");

            var builder = WebApplication.CreateBuilder(args);
            builder.Configuration
               .SetBasePath(Directory.GetCurrentDirectory())
               .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
               .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true)
               .AddEnvironmentVariables();

            builder.Host.UseSerilog((ctx, services, configuration) =>
            {
                configuration
                    .Enrich.FromLogContext()
                    .WriteTo.Console(theme: AnsiConsoleTheme.Code);
            });

            var startup = new Startup(builder.Configuration);
            startup.ConfigureServices(builder.Services);

            var app = builder.Build();
            startup.ConfigureApp(app, app.Environment);

            Log.Logger.Information("Successfully configured services, running the app.");
            app.Run();
        }
        catch (Exception exception)
        {
            Log.Logger.Error(exception, "An error occurred during application startup.");
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }
}

