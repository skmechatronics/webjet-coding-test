using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using WebJet.Entertainment.Services.ApiModels;
using WebJet.Entertainment.Services.Helpers;
using WebJet.Entertainment.Services.Interfaces;
using WebJet.Entertainment.Services.UiModels;

namespace WebJet.Entertainment.Services;

public class MoviesAggregatorService : IMoviesAggregatorService
{
    private readonly ILogger<MoviesAggregatorService> _logger;
    private readonly IMoviesCacheService _moviesCacheService;
    private readonly Dictionary<string, IMoviesHttpClient> _clientMap;

    public MoviesAggregatorService(
        ILogger<MoviesAggregatorService> logger,
        IEnumerable<IMoviesHttpClient> movieClients,
        IMoviesCacheService moviesCacheService)
    {
        _logger = logger;
        _moviesCacheService = moviesCacheService;
        _clientMap = movieClients.ToDictionary(c => c.Source.Name);
    }

    public async Task<MoviesCollectionModel> GetAllMovies()
    {
        var sourceToMoviesCollection = new ConcurrentDictionary<string, MoviesCollection>();

        var getMoviesTasks = _clientMap.Values
            .Select(client => FetchFromCacheOrHttpAndStoreAsync(client, sourceToMoviesCollection))
            .ToList();

        await Task.WhenAll(getMoviesTasks);

        var flattened = sourceToMoviesCollection
            .SelectMany(sourceToMovie =>
                sourceToMovie.Value.Movies.Select(movie => (Source: sourceToMovie.Key, Movie: movie)));

        var groupedByTitle = flattened
            .GroupBy(entry => entry.Movie.Title, StringComparer.OrdinalIgnoreCase);

        var mergedMovies = groupedByTitle
            .Select(group =>
            {
                var original = group.First().Movie;
                var sources = group.Select(entry => entry.Source).Distinct().ToList();
                return original.MapToModel(sources);
            })
            .ToList();

        return new MoviesCollectionModel { Movies = mergedMovies };
    }

    public async Task<Result<MovieMetadataModel>> GetMovieMetadataByPriceAsync(string title)
    {
        var allSources = await GetMovieMetadataFromAllSourcesAsync(title);

        if (allSources.Count == 0)
        {
            return new ErrorResult<MovieMetadataModel>
            {
                ErrorCode = ErrorCodes.NoMoviesFound,
                ErrorMessage = "No movies were found for this title."
            };
        }

        var baseMetadata = allSources.First().Metadata;
        var model = baseMetadata.MapToModel();

        model.SourcePrices = allSources
            .Select(pair => new SourcePrice
            {
                Source = pair.Source.Name,
                Price = pair.Metadata.Price
            })
            .OrderBy(sp => sp.Price)
            .ToList();

        return new SuccessResult<MovieMetadataModel> { Data = model };
    }

    private async Task<List<(MovieSource Source, MovieMetadata Metadata)>> GetMovieMetadataFromAllSourcesAsync(string title)
    {
        var tasks = _clientMap.Values.Select(async client =>
        {
            var result = await GetMovieMetadataBySourceAsync(client.Source, title);
            if (result is SuccessResult<MovieMetadata> success)
            {
                return (Success: true, Pair: (client.Source, success.Data));
            }

            if (result is ErrorResult<MovieMetadata> error)
            {
                _logger.LogError(error.Exception,
                    "Failed to get metadata for '{Title}' from source {Source}: {Message}",
                    title, client.Source, error.ErrorMessage);
            }

            return (Success: false, Pair: default);
        });

        var results = await Task.WhenAll(tasks);

        return results
            .Where(r => r.Success)
            .Select(r => r.Pair)
            .ToList();
    }

    private async Task<Result<MovieMetadata>> GetMovieMetadataBySourceAsync(MovieSource source, string title)
    {
        _logger.LogInformation("Attempting to retrieve metadata for '{Title}' from cache ({MovieSource})", title, source.Name);

        var movieMetadata = await _moviesCacheService.TryGetMovieMetadataFromCache(source, title);
        if (movieMetadata is SuccessResult<MovieMetadata> cachedSuccessResult)
        {
            _logger.LogInformation("Cache hit for metadata of '{Title}' from {MovieSource}", title, source.Name);
            return cachedSuccessResult;
        }

        _logger.LogWarning("Cache miss for metadata of '{Title}' from {MovieSource}", title, source.Name);

        var movieResult = await GetMovieByTitleAsync(source, title);
        if (movieResult is ErrorResult<Movie> errorResult)
        {
            _logger.LogWarning("Failed to resolve movie '{Title}' to ID from {MovieSource}", title, source.Name);
            return errorResult.MapError<Movie, MovieMetadata>();
        }

        var successResult = movieResult as SuccessResult<Movie>;
        if (successResult is null)
        {
            _logger.LogError("Unexpected null resolution for movie '{Title}' from {MovieSource}", title, source.Name);
            return new ErrorResult<MovieMetadata>
            {
                ErrorMessage = $"Unexpected null result for '{title}' from {source.Name}"
            };
        }

        var externalClient = _clientMap[source.Name];
        _logger.LogInformation("Fetching metadata from external API for '{Title}' ({MovieSource})", title, source.Name);

        var retrievedMetadata = await externalClient.GetMovieById(successResult.Data.Id);
        if (retrievedMetadata is SuccessResult<MovieMetadata> successMetadataResult
            && successMetadataResult.Data is not null)
        {
            _logger.LogInformation("Successfully fetched metadata from external API for '{Title}' ({MovieSource}), caching result", title, source.Name);
            await _moviesCacheService.CacheMovieMetadata(source, successMetadataResult.Data);
        }

        return retrievedMetadata;
    }


    private async Task<Result<Movie>> GetMovieByTitleAsync(MovieSource source, string title)
    {
        _logger.LogInformation("Looking up movie '{Title}' from source {MovieSource}", title, source);

        var cacheResult = await _moviesCacheService.TryGetMovieTitleFromCache(source, title);
        if (cacheResult is SuccessResult<Movie> cached)
        {
            _logger.LogInformation("Cache hit for movie '{Title}' from {MovieSource}", title, source);
            return cached;
        }

        _logger.LogWarning("Cache miss for movie '{Title}' from {MovieSource}", title, source);

        if (!_clientMap.TryGetValue(source.Name, out var client))
        {
            _logger.LogError("No client found for source {MovieSource}", source);
            return new ErrorResult<Movie>
            {
                ErrorMessage = $"Client not found for source: {source.Name}",
                ErrorCode = ErrorCodes.MovieSourceNotConfigured
            };
        }

        var tempStore = new ConcurrentDictionary<string, MoviesCollection>();
        await FetchFromExternalProxyAsync(client, tempStore);

        if (!tempStore.TryGetValue(source.Name, out var collection))
        {
            return new ErrorResult<Movie>
            {
                ErrorMessage = $"No movies returned from external source for {source.Name}",
                ErrorCode = ErrorCodes.MovieSourceNotAvailable
            };
        }

        var matchedMovie = collection.Movies
            .FirstOrDefault(m => string.Equals(m.Title, title, StringComparison.OrdinalIgnoreCase));

        return matchedMovie is not null
            ? new SuccessResult<Movie> { Data = matchedMovie }
            : new ErrorResult<Movie>
            {
                ErrorMessage = $"Movie '{title}' not found in fetched data for source {source.Name}",
                ErrorCode = ErrorCodes.MovieNotFound
            };
    }

    private async Task FetchFromCacheOrHttpAndStoreAsync(
        IMoviesHttpClient client,
        ConcurrentDictionary<string, MoviesCollection> store)
    {
        _logger.LogInformation("Trying cache for source {MovieSource}", client.Source.Name);

        var cacheResult = await _moviesCacheService.TryGetCollectionFromCache(client.Source);
        if (cacheResult is SuccessResult<MoviesCollection> cached)
        {
            _logger.LogInformation("Cache hit for source {MovieSource}", client.Source.Name);
            store.TryAdd(client.Source.Name, cached.Data);
            return;
        }

        await FetchFromExternalProxyAsync(client, store);
    }

    private async Task FetchFromExternalProxyAsync(
        IMoviesHttpClient client,
        ConcurrentDictionary<string, MoviesCollection> store)
    {
        _logger.LogInformation("Cache miss or error, falling back to HTTP for {MovieSource}", client.Source);

        var httpResult = await client.GetMoviesCollection();
        if (httpResult is SuccessResult<MoviesCollection> fetched)
        {
            _logger.LogInformation("Fetched {Count} movies from HTTP for {MovieSource}",
                fetched.Data.Movies.Count, client.Source);

            store.TryAdd(client.Source.Name, fetched.Data);
            await _moviesCacheService.CacheMoviesCollection(client.Source, fetched.Data);
        }
        else if (httpResult is ErrorResult<MoviesCollection> error)
        {
            _logger.LogError(error.Exception, "Failed to retrieve movies from HTTP source {MovieSource}", client.Source);
        }
    }
}
