using System.Text.Json.Serialization;
using WebJet.Entertainment.Services.ExternalProxy;

namespace WebJet.Entertainment.Services.ApiModels;

public record Movie
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
