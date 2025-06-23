using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Net.Mime;

namespace WebJet.Entertainment.Api.HealthChecks;

public static class HealthCheckExtensions
{
    public static HealthCheckOptions CreateResponse(string healthCheckName)
    {
        async Task responseWriter(HttpContext context, HealthReport report)
        {
            context.Response.ContentType = MediaTypeNames.Text.Plain;
            var healthCheckResponse = report.Entries[healthCheckName].Description ?? string.Empty;
            await context.Response.WriteAsync(healthCheckResponse);
        }

        return new HealthCheckOptions
        {
            ResponseWriter = responseWriter
        };
    }
}
