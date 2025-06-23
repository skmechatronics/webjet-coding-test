using System.Text.Json.Serialization;
using WebJet.Entertainment.Services.ExternalProxy;

namespace WebJet.Entertainment.Services.ApiModels;

public record MovieMetadata
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
