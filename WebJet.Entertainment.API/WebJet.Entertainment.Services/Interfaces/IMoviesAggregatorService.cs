using WebJet.Entertainment.Services.UiModels;

namespace WebJet.Entertainment.Services.Interfaces;

public interface IMoviesAggregatorService
{
    public Task<MoviesCollectionModel> GetAllMovies();

    public Task<Result<MovieMetadataModel>> GetMovieMetadataByPriceAsync(string title);
}
