namespace WebJet.Entertainment.PoC;

internal static class Utilities
{
    public static bool AnyStringsNullOrEmpty(params string?[] values)
    {
        return values.Select(i => i?.Trim()).Any(string.IsNullOrEmpty);
    }

    public static string EnsureTrailingSlash(this string url)
    {
        return url.EndsWith("/") ? url : url + "/";
    }
}
