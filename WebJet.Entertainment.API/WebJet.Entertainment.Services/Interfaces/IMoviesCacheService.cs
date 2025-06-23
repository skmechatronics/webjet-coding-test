using WebJet.Entertainment.Services.ApiModels;
using WebJet.Entertainment.Services.UiModels;

namespace WebJet.Entertainment.Services
{
    public interface IMoviesCacheService
    {
        Task CacheMoviesCollection(MovieSource movieSource, MoviesCollection moviesCollection);

        Task CacheMovieMetadata(MovieSource movieSource, MovieMetadata movieMetadata);

        Task<Result<MoviesCollection>> TryGetCollectionFromCache(MovieSource movieSource);

        Task<Result<Movie>> TryGetMovieTitleFromCache(MovieSource movieSource, string title);

        Task<Result<MovieMetadata>> TryGetMovieMetadataFromCache(MovieSource movieSource, string title);
    }
}