using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using WebJet.Entertainment.Services.ApiModels;
using WebJet.Entertainment.Services.Configuration;
using WebJet.Entertainment.Services.Interfaces;
using WebJet.Entertainment.Services.UiModels;

namespace WebJet.Entertainment.Services.ExternalProxy;

public class FilmworldHttpClient : BaseHttpClient, IMoviesHttpClient
{
    private const string MovieCollectionEndpoint = "movies";
    private const string IndividualMovieEndpoint = "movie";

    public FilmworldHttpClient(
        IHttpClientFactory httpClientFactory,
        IOptions<WebJetConfiguration> options,
        ILogger<FilmworldHttpClient> logger)
        : base(httpClientFactory,
              logger)
    {
        BaseUrl = options.Value.FilmworldApiBaseUrl!;
        ApiKey = options.Value.ApiKey!;
    }

    public MovieSource Source => MovieSource.Filmworld;

    public Task<Result<MoviesCollection>> GetMoviesCollection() =>
        TryGetResponse<MoviesCollection>(MovieCollectionEndpoint);

    public Task<Result<MovieMetadata>> GetMovieById(string id) =>
        TryGetResponse<MovieMetadata>($"{IndividualMovieEndpoint}/{id}");
}
