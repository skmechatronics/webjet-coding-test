using WebJet.Entertainment.Services.ApiModels;
using WebJet.Entertainment.Services.UiModels;

namespace WebJet.Entertainment.Services.Helpers
{
    internal static class Mappers
    {
        public static MovieModel MapToModel(this Movie original, List<string> sources)
        {
            return new MovieModel
            {
                Title = original.Title,
                Year = original.Year,
                PosterUrl = original.PosterUrl,
                AvailableSources = sources
            };
        }

        public static MovieMetadataModel MapToModel(this MovieMetadata movieMetadata)
        {
            return new MovieMetadataModel
            {
                Title = movieMetadata.Title,
                Year = movieMetadata.Year,
                Rated = movieMetadata.Rated,
                Released = movieMetadata.Released,
                Runtime = movieMetadata.Runtime,
                Genre = movieMetadata.Genre,
                Director = movieMetadata.Director,
                Writer = movieMetadata.Writer,
                Actors = movieMetadata.Actors,
                Plot = movieMetadata.Plot,
                Language = movieMetadata.Language,
                Country = movieMetadata.Country,
                Awards = movieMetadata.Awards,
                PosterUrl = movieMetadata.Poster,
                Metascore = movieMetadata.Metascore,
                Rating = movieMetadata.Rating,
                Votes = movieMetadata.Votes,
                SourcePrices = []
            };
        }
    }
}
