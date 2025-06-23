namespace WebJet.Entertainment.Services.UiModels;

// Ensure these do not overlap with HTTP codes
public static class ErrorCodes
{
    public const int FatalError = 600;

    public const int JsonSerialisationError = 601;

    public const int CacheValueNullError = 700;

    public const int CacheSerializationError = 701;

    public const int MovieSourceNotConfigured = 702;

    public const int MovieSourceNotAvailable = 703;

    public const int MovieNotFound = 704;

    public const int NoMoviesFound = 705;

}
