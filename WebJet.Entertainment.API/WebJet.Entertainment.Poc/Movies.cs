using System.Text.Json.Serialization;

namespace WebJet.Entertainment.Poc;

public record MovieCollection
{
    public List<CinemaworldMovie> Movies { get; set; }
}

public record CinemaworldMovie
{
    [JsonPropertyName("ID")]
    public string Id { get; set; }

    public string Title { get; set; }

    [JsonConverter(typeof(IntConverter))]
    public int Year { get; set; }

    public string Type { get; set; }

    [JsonPropertyName("Poster")]
    public string PosterUrl { get; set; }
}


public record MovieDetails
{
    public string Title { get; set; }

    [JsonConverter(typeof(IntConverter))]
    public int Year { get; set; }

    public string Rated { get; set; }

    [JsonConverter(typeof(DateTimeConverter))]
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

    public string Poster { get; set; }

    [JsonConverter(typeof(IntConverter))]
    public int Metascore { get; set; }

    [JsonConverter(typeof(DecimalConverter))]
    public decimal Rating { get; set; }

    [JsonConverter(typeof(IntConverter))]
    public int Votes { get; set; }

    public string ID { get; set; }

    public string Type { get; set; }

    [JsonConverter(typeof(DecimalConverter))]
    public decimal Price { get; set; }
}
