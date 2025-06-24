using Microsoft.Extensions.Diagnostics.HealthChecks;
using WebJet.Entertainment.Services.Interfaces;
using WebJet.Entertainment.Services.ApiModels;
using WebJet.Entertainment.Services.UiModels;
using System.Net.Mime;

namespace WebJet.Entertainment.Api.HealthChecks;

public sealed record HealthStatus(string Status, string Message);

public class AdvancedHealthChecks : IHealthCheck
{
    public const string Tag = "advanced";

    private const string StatusHealthy = "Healthy";
    private const string StatusUnhealthy = "Unhealthy";

    private readonly ILogger<AdvancedHealthChecks> _logger;
    private readonly IEnumerable<IMoviesHttpClient> _clients;

    public AdvancedHealthChecks(
        ILogger<AdvancedHealthChecks> logger,
        IEnumerable<IMoviesHttpClient> clients)
    {
        _logger = logger;
        _clients = clients;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        var data = new Dictionary<string, object>();
        var healthyCount = 0;

        foreach (var client in _clients)
        {
            var source = client.Source.ToString();
            var result = await CheckClientAsync(client);

            if (result is SuccessResult<HealthStatus> success)
            {
                if (success.Data.Status == StatusHealthy)
                {
                    healthyCount++;
                }

                data[source] = success.Data.Message;
            }
            else if (result is ErrorResult<HealthStatus> error)
            {
                data[source] = error.ErrorMessage;
            }
        }

        var total = _clients.Count();

        if (healthyCount == total)
        {
            return HealthCheckResult.Healthy("All sources healthy", data);
        }

        if (healthyCount > 0)
        {
            return HealthCheckResult.Degraded("Some sources failed", data: data);
        }

        return HealthCheckResult.Unhealthy("All sources failed", data: data);
    }

    private async Task<Result<HealthStatus>> CheckClientAsync(IMoviesHttpClient client)
    {
        try
        {
            var collection = await client.GetMoviesCollection();
            if (collection is not SuccessResult<MoviesCollection> success || !success.Data.Movies.Any())
            {
                return new ErrorResult<HealthStatus>
                {
                    ErrorMessage = "Failed to fetch movies collection."
                };
            }

            var firstId = success.Data.Movies.First().Id;
            var detail = await client.GetMovieById(firstId);

            if (detail is SuccessResult<MovieMetadata>)
            {
                return new SuccessResult<HealthStatus>
                {
                    Data = new HealthStatus(StatusHealthy, "Healthy")
                };
            }
            else
            {
                return new ErrorResult<HealthStatus>
                {
                    ErrorMessage = "Failed to fetch metadata."
                };
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception during health check for {Source}", client.Source);

            return new ErrorResult<HealthStatus>
            {
                ErrorMessage = $"Exception: {ex.Message}",
                Exception = ex
            };
        }
    }

    public static Task ResponseWriter(HttpContext context, HealthReport report)
    {
        context.Response.ContentType = MediaTypeNames.Application.Json;

        var entry = report.Entries.First().Value;

        var result = new
        {
            status = report.Status.ToString(),
            sources = entry.Data.Select(kvp => new
            {
                source = kvp.Key,
                status = kvp.Value.ToString() == StatusHealthy ? StatusHealthy : StatusUnhealthy,
                message = kvp.Value.ToString()
            })
        };

        return context.Response.WriteAsJsonAsync(result);
    }
}

