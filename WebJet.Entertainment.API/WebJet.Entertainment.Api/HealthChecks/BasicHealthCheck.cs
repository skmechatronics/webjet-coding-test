using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Net.Mime;

namespace WebJet.Entertainment.Api.HealthChecks;

// A very simple health check for applications and other services
// to ping to figure out if this API is healthy.
// It can also be used to figure out if this application was successfully
// deployed and used by Kubernetes health/liveness checks.
public class BasicHealthCheck : IHealthCheck
{
    public const string Tag = "basic";
    private const string BuildNumberKey = "BUILD_NUMBER";
    private const string DefaultBuildNumber = "dev";

    // default build number for development purposes
    private readonly string BuildNumber = DefaultBuildNumber;

    public BasicHealthCheck(IHostEnvironment hostEnvironment, IConfiguration configuration)
    {
        if (!hostEnvironment.IsDevelopment())
        {
            BuildNumber = configuration.GetSection(BuildNumberKey).Value ?? string.Empty;
        }
    }

    public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        var healthyStatus = HealthCheckResult.Healthy(BuildNumber);
        return Task.FromResult(healthyStatus);
    }

    public static Task ResponseWriter(HttpContext context, HealthReport report)
    {
        context.Response.ContentType = MediaTypeNames.Application.Json;
        var build = report.Entries.Values.FirstOrDefault().Description ?? DefaultBuildNumber;
        return context.Response.WriteAsJsonAsync(new { build });
    }
}
