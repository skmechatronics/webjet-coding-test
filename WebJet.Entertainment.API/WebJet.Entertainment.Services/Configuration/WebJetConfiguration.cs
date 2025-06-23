namespace WebJet.Entertainment.Services.Configuration;

public record WebJetConfiguration
{
    public const string SectionName = nameof(WebJetConfiguration);

    public const string ConfigurationMissingMessage = "Configuration is missing, please see README and check your configuration.";

    public string? FilmworldApiBaseUrl { get; set; }

    public string? CinemaworldApiBaseUrl { get; set; }

    public string? ApiKey { get; set; }

    public bool IsValid()
    {
        return Uri.IsWellFormedUriString(CinemaworldApiBaseUrl, UriKind.Absolute)
            && Uri.IsWellFormedUriString(FilmworldApiBaseUrl, UriKind.Absolute)
            && !string.IsNullOrEmpty(ApiKey?.Trim());
    }
}
