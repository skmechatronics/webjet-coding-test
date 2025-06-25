using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.OpenApi.Models;
using WebJet.Entertainment.Api.HealthChecks;
using WebJet.Entertainment.Services;
using WebJet.Entertainment.Services.Configuration;
using WebJet.Entertainment.Services.ExternalProxy;
using WebJet.Entertainment.Services.Interfaces;

namespace WebJet.Entertainment.Api;

public class Startup
{
    private const string DevCorsPolicy = nameof(DevCorsPolicy);
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
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Version = "v1",
                Title = "WebJet Entertainment API",
                Description = "In-flight streaming entertainment, for your viewing pleasure.",
            });
            options.EnableAnnotations();
        });

        services.AddCors(options =>
        {
            options.AddPolicy(DevCorsPolicy, policy =>
            {
                policy.WithOrigins("*")
                      .AllowAnyHeader()
                      .AllowAnyMethod();
            });
        });

        services.AddHealthChecks()
            .AddCheck<BasicHealthCheck>(nameof(BasicHealthCheck), tags: [BasicHealthCheck.Tag]);

        services.AddHealthChecks()
            .AddCheck<AdvancedHealthChecks>(nameof(AdvancedHealthChecks), tags: [AdvancedHealthChecks.Tag]);
    }

    public void ConfigureApp(WebApplication app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseSwagger(options => options.OpenApiVersion = Microsoft.OpenApi.OpenApiSpecVersion.OpenApi3_0);
            app.UseSwaggerUI();
            app.UseCors(DevCorsPolicy   );
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
