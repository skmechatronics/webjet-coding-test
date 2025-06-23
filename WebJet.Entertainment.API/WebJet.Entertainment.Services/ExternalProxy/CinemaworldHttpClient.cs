using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using WebJet.Entertainment.Services.ApiModels;
using WebJet.Entertainment.Services.Configuration;
using WebJet.Entertainment.Services.Interfaces;
using WebJet.Entertainment.Services.UiModels;

namespace WebJet.Entertainment.Services.ExternalProxy;

public class CinemaworldHttpClient : BaseHttpClient, IMoviesHttpClient
{
    private const string MovieCollectionEndpoint = "movies";
    private const string IndividualMovieEndpoint = "movie";

    public CinemaworldHttpClient(
        IHttpClientFactory httpClientFactory,
        IOptions<WebJetConfiguration> options,
        ILogger<CinemaworldHttpClient> logger)
    : base(httpClientFactory, logger)
    {
        BaseUrl = options.Value.CinemaworldApiBaseUrl!;
        ApiKey = options.Value.ApiKey!;
    }

    public MovieSource Source => MovieSource.Cinemaworld;

    public Task<Result<MoviesCollection>> GetMoviesCollection() =>
        TryGetResponse<MoviesCollection>(MovieCollectionEndpoint);

    public Task<Result<MovieMetadata>> GetMovieById(string id) =>
        TryGetResponse<MovieMetadata>($"{IndividualMovieEndpoint}/{id}");
}
