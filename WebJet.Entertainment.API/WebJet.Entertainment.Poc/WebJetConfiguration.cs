using WebJet.Entertainment.PoC;

namespace WebJet.Entertainment.Poc;

public class WebJetConfiguration
{
    public const string SectionName = nameof(WebJetConfiguration);

    public string? FilmworldApiBaseUrl { get; set; }

    public string? CinemaworldApiBaseUrl { get; set; }

    public string? ApiKey { get; set; }

    public bool IsMisconfigured()
    {
        return Utilities.AnyStringsNullOrEmpty(FilmworldApiBaseUrl, CinemaworldApiBaseUrl, ApiKey);
    }
}
