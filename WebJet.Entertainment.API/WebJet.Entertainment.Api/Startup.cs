using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using WebJet.Entertainment.Api.HealthChecks;
using WebJet.Entertainment.Services;
using WebJet.Entertainment.Services.Configuration;
using WebJet.Entertainment.Services.ExternalProxy;
using WebJet.Entertainment.Services.Interfaces;

namespace WebJet.Entertainment.Api;

public class Startup
{
    private readonly IConfiguration _configuration;

    public Startup(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public void ConfigureServices(IServiceCollection services)
    {
        ConfigureOptions(services);
        ConfigureApplicationServices(services);
        ConfigureInfastructure(services);
    }

    private void ConfigureOptions(IServiceCollection services)
    {
        services
        .AddOptions<WebJetConfiguration>()
        .Bind(_configuration.GetSection(WebJetConfiguration.SectionName))
        .Validate(cfg => cfg.IsValid(), WebJetConfiguration.ConfigurationMissingMessage)
        .ValidateOnStart();
    }

    private void ConfigureApplicationServices(IServiceCollection services)
    {
        services.AddHttpClient();
        services.AddSingleton<IMoviesCacheService, MoviesCacheService>();
        services.AddSingleton<IMoviesHttpClient, CinemaworldHttpClient>();
        services.AddSingleton<IMoviesHttpClient, FilmworldHttpClient>();
        services.AddSingleton<IMoviesAggregatorService, MoviesAggregatorService>();
    }

    private void ConfigureInfastructure(IServiceCollection services)
    {
        // See README for a discussion on this.
        services.AddDistributedMemoryCache();
        services.AddControllers();
        services.Configure<RouteOptions>(options =>
        {
            options.LowercaseUrls = true;
        });
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();

        services.AddHealthChecks() // First group
            .AddCheck<BasicHealthCheck>(nameof(BasicHealthCheck), tags: [BasicHealthCheck.Tag]);

        services.AddHealthChecks()
            .AddCheck<AdvancedHealthChecks>(nameof(AdvancedHealthChecks), tags: [AdvancedHealthChecks.Tag]);
    }

    public void ConfigureApp(WebApplication app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();
        app.UseAuthorization();
        app.MapControllers();

        app.MapHealthChecks("/health", new HealthCheckOptions
        {
            Predicate = check => check.Tags.Contains(BasicHealthCheck.Tag),
            ResponseWriter = BasicHealthCheck.ResponseWriter
        });

        app.MapHealthChecks("/external-health", new HealthCheckOptions
        {
            Predicate = check => check.Tags.Contains(AdvancedHealthChecks.Tag),
            ResponseWriter = AdvancedHealthChecks.ResponseWriter
        });
    }
}
