using WebJet.Entertainment.Services.UiModels;

namespace WebJet.Entertainment.Services.Helpers;

internal static class Utilities
{
    private const string UrlSlash = "/";

    public static bool AreAllValidUrls(params string?[] values)
    {
        return values.All(i => Uri.IsWellFormedUriString(i, UriKind.Absolute));
    }

    // When configuring URL's ensure there is a trailing slash
    // otherwise the last path segment gets overwritten in a get request
    public static string EnsureTrailingSlash(this string url)
    {
        if (!url.EndsWith(UrlSlash))
        {
            url += UrlSlash;
        }

        return url;
    }

    public static ErrorResult<U> MapError<T, U>(this ErrorResult<T> error)
    {
        return new ErrorResult<U>
        {
            ErrorCode = error.ErrorCode,
            ErrorMessage = error.ErrorMessage,
            Exception = error.Exception
        };
    }

}
