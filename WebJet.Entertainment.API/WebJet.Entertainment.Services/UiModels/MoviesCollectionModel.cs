namespace WebJet.Entertainment.Services.UiModels
{
    public class MoviesCollectionModel
    {
        public List<MovieModel> Movies { get; set; } = [];
    }

    public class MovieModel
    {
        public string Title { get; set; }

        public int Year { get; set; }
            
        public string PosterUrl { get; set; }

        public List<string> AvailableSources { get; set; }
    }

    public class MovieMetadataModel
    {
        public string Title { get; set; }

        public int Year { get; set; }

        public string Rated { get; set; }

        public DateTime Released { get; set; }

        public string Runtime { get; set; }

        public string Genre { get; set; }

        public string Director { get; set; }

        public string Writer { get; set; }

        public string Actors { get; set; }

        public string Plot { get; set; }

        public string Language { get; set; }

        public string Country { get; set; }

        public string Awards { get; set; }

        public string PosterUrl { get; set; }

        public int Metascore { get; set; }

        public decimal Rating { get; set; }

        public int Votes { get; set; }

        public List<SourcePrice> SourcePrices { get; set; } = [];
    }

    public record SourcePrice
    {
        public string Source { get; set; }

        public decimal Price { get; set; }
    }
}
