namespace Hyde.Extensions;

internal static class StringExtensions
{
    public static string Sluggify(this string value)
    {
        return value
            .ToLowerInvariant()
            .Replace(' ', '-')
            .Replace('\'', '-');
    }
}
