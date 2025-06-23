using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net.Http.Json;
using System.Text.Json;
using WebJet.Entertainment.Services.ApiModels;
using WebJet.Entertainment.Services.UiModels;

namespace WebJet.Entertainment.Services.ExternalProxy;

public abstract class BaseHttpClient
{
    private const string AccessTokenHeaderKey = "x-access-token";

    public BaseHttpClient(IHttpClientFactory httpClientFactory, ILogger<BaseHttpClient> logger)
    {
        HttpClientFactory = httpClientFactory;
        _logger = logger;
    }

    protected IHttpClientFactory HttpClientFactory { get; init; }

    protected string BaseUrl { get; init; }

    protected string ApiKey { get; init; }

    protected TimeSpan Timeout { get; init; } = TimeSpan.FromSeconds(10);

    private ILogger<BaseHttpClient> _logger;

    protected HttpClient CreateClient()
    {
        var httpClient = HttpClientFactory.CreateClient();
        httpClient.BaseAddress = new Uri(BaseUrl);
        httpClient.DefaultRequestHeaders.Add(AccessTokenHeaderKey, ApiKey);
        httpClient.Timeout = Timeout;
        return httpClient;
    }

    protected async Task<Result<T>> TryGetResponse<T>(string endpoint)
    {
        try
        {
            var httpClient = CreateClient();
            return await GetResponse<T>(httpClient, endpoint);
        }
        catch (JsonException jsonException)
        {
            return new ErrorResult<T>
            {
                ErrorCode = ErrorCodes.JsonSerialisationError,
                ErrorMessage = jsonException.Message,
                Exception = jsonException
            };
        }
        catch (Exception exception)
        {
            return new ErrorResult<T>
            {
                ErrorCode = ErrorCodes.FatalError,
                ErrorMessage = exception.Message,
                Exception = exception
            };
        }
    }

    private async Task<Result<T>> GetResponse<T>(HttpClient httpClient, string endpoint)
    {
        _logger.LogInformation("Calling API {API}/{Endpoint}", httpClient.BaseAddress, endpoint);
        var response = await httpClient.GetAsync(endpoint);
        if (!response.IsSuccessStatusCode)
        {
            var additionalDetails = await response.Content.ReadAsStringAsync() ?? "No further details.";
            return new ErrorResult<T>
            {
                ErrorCode = (int)response.StatusCode,
                ErrorMessage = $"An error occurred fetching content: {additionalDetails}"
            };
        }

        var rawContent = await response.Content.ReadAsStringAsync();
        _logger.LogDebug("Raw API response: {RawContent}", rawContent);

        var content = JsonSerializer.Deserialize<T>(rawContent);
        if (content is null)
        {
            return new ErrorResult<T>
            {
                ErrorCode = ErrorCodes.JsonSerialisationError,
                ErrorMessage = "Failed to deserialize content",
                Exception = new InvalidOperationException("Expected non-null content but received null.")
            };
        }

        return new SuccessResult<T> { Data = content };
    }
}
