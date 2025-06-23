using WebJet.Entertainment.Services.ApiModels;
using WebJet.Entertainment.Services.UiModels;

namespace WebJet.Entertainment.Services.Interfaces;

public interface IMoviesHttpClient
{
    public Task<Result<MoviesCollection>> GetMoviesCollection();

    public Task<Result<MovieMetadata>> GetMovieById(string id);

    public MovieSource Source { get; }
}
