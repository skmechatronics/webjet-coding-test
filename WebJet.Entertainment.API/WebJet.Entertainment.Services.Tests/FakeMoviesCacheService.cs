using System.Collections.Concurrent;
using WebJet.Entertainment.Services.ApiModels;
using WebJet.Entertainment.Services.UiModels;

namespace WebJet.Entertainment.Services.Tests
{
    internal class FakeMoviesCacheService : IMoviesCacheService
    {
        public ConcurrentDictionary<string, MoviesCollection> Collections { get; } = new();
        public ConcurrentDictionary<(string source, string title), Movie> MovieCache { get; } = new();
        public ConcurrentDictionary<(string source, string title), MovieMetadata> MetadataCache { get; } = new();

        public Task CacheMoviesCollection(MovieSource source, MoviesCollection collection)
        {
            Collections[source.Name] = collection;
            foreach (var movie in collection.Movies)
            {
                MovieCache[(source.Name, movie.Title)] = movie;
            }
            return Task.CompletedTask;
        }

        public Task<Result<MoviesCollection>> TryGetCollectionFromCache(MovieSource source)
        {
            if (Collections.TryGetValue(source.Name, out var collection))
            {
                return Task.FromResult<Result<MoviesCollection>>(new SuccessResult<MoviesCollection> { Data = collection });
            }

            return Task.FromResult<Result<MoviesCollection>>(new ErrorResult<MoviesCollection>());
        }

        public Task<Result<Movie>> TryGetMovieTitleFromCache(MovieSource source, string title)
        {
            if (MovieCache.TryGetValue((source.Name, title), out var movie))
            {
                return Task.FromResult<Result<Movie>>(new SuccessResult<Movie> { Data = movie });
            }

            return Task.FromResult<Result<Movie>>(new ErrorResult<Movie>());
        }

        public Task CacheMovieMetadata(MovieSource source, MovieMetadata metadata)
        {
            MetadataCache[(source.Name, metadata.Title)] = metadata;
            return Task.CompletedTask;
        }

        public Task<Result<MovieMetadata>> TryGetMovieMetadataFromCache(MovieSource source, string title)
        {
            if (MetadataCache.TryGetValue((source.Name, title), out var metadata))
            {
                return Task.FromResult<Result<MovieMetadata>>(new SuccessResult<MovieMetadata> { Data = metadata });
            }

            return Task.FromResult<Result<MovieMetadata>>(new ErrorResult<MovieMetadata>());
        }
    }
}