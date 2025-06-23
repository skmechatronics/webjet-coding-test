using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using WebJet.Entertainment.Services.ApiModels;
using WebJet.Entertainment.Services.UiModels;

namespace WebJet.Entertainment.Services
{
    public class MoviesCacheService : IMoviesCacheService
    {
        private const string MetadataPrefix = "METADATA";
        private const string CollectionPrefix = "COLLECTION";
        private const string MoviePrefix = "MOVIE";
        private const string Separator = "_";

        private readonly ILogger<MoviesCacheService> _logger;
        private readonly IDistributedCache _distributedCache;

        private string CreateCollectionCacheKey(MovieSource movieSource) => string.Join(Separator, CollectionPrefix, movieSource.Name);
        private string CreateMovieCacheKey(MovieSource movieSource, string title) => string.Join(Separator, MoviePrefix, movieSource.Name, title);
        private string CreateMetadataCacheKey(MovieSource movieSource, string title) => string.Join(Separator, MetadataPrefix, movieSource.Name, title);


        private static readonly DistributedCacheEntryOptions MetadataCacheOptions = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1)
        };

        private static readonly DistributedCacheEntryOptions CollectionCacheOptions = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
        };

        public MoviesCacheService(
            ILogger<MoviesCacheService> logger,
            IDistributedCache distributedCache)
        {
            _logger = logger;
            _distributedCache = distributedCache;
        }

        public async Task CacheMoviesCollection(MovieSource movieSource, MoviesCollection moviesCollection)
        {
            var cacheKey = CreateCollectionCacheKey(movieSource);
            var cachedData = JsonSerializer.SerializeToUtf8Bytes(moviesCollection);
            await _distributedCache.SetAsync(cacheKey, cachedData, CollectionCacheOptions);
            await CacheIndividualMovie(movieSource, moviesCollection);
        }

        public async Task CacheMovieMetadata(MovieSource movieSource, MovieMetadata movieMetadata)
        {
            var cacheKey = CreateMetadataCacheKey(movieSource, movieMetadata.Title);
            var cachedData = JsonSerializer.SerializeToUtf8Bytes(movieMetadata);
            await _distributedCache.SetAsync(cacheKey, cachedData, MetadataCacheOptions);
        }

        private async Task CacheIndividualMovie(MovieSource movieSource, MoviesCollection moviesCollection)
        {
            foreach(var movie in moviesCollection.Movies)
            {
                var cacheKey = $"{MoviePrefix}_{movieSource.Name}_{movie.Title}";
                var cachedData = JsonSerializer.SerializeToUtf8Bytes(movie);
                await _distributedCache.SetAsync(cacheKey, cachedData, CollectionCacheOptions);
            }
        }

        public Task<Result<MoviesCollection>> TryGetCollectionFromCache(MovieSource movieSource)
        {
            var cacheKey = CreateCollectionCacheKey(movieSource);
            return TryGetFromCache<MoviesCollection>(cacheKey);
        }

        public Task<Result<Movie>> TryGetMovieTitleFromCache(MovieSource movieSource, string title)
        {
            var cacheKey = CreateMovieCacheKey(movieSource, title);
            return TryGetFromCache<Movie>(cacheKey);
        }

        public Task<Result<MovieMetadata>> TryGetMovieMetadataFromCache(MovieSource movieSource, string title)
        {
            var cacheKey = CreateMetadataCacheKey(movieSource, title);
            return TryGetFromCache<MovieMetadata>(cacheKey);
        }

        private async Task<Result<T>> TryGetFromCache<T>(string cacheKey)
        {
            try
            {
                var data = await _distributedCache.GetAsync(cacheKey);
                if (data is null)
                {
                    _logger.LogInformation("Cache miss for key {CacheKey}", cacheKey);
                    return new ErrorResult<T> { ErrorMessage = "Cache miss" };
                }

                var deserialized = JsonSerializer.Deserialize<T>(data);
                return deserialized is not null
                    ? new SuccessResult<T> { Data = deserialized }
                    : new ErrorResult<T> { 
                        ErrorMessage = "Cache corruption: deserialized to null.",
                        ErrorCode = ErrorCodes.CacheValueNullError};
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "Deserialization failed for cache key {CacheKey}", cacheKey);
                return new ErrorResult<T> { 
                    ErrorMessage = "Cache corruption: deserialization exception.", 
                    ErrorCode = ErrorCodes.CacheSerializationError,
                    Exception = ex };
            }
        }

    }
}
