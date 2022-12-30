namespace Hyde.Extensions;

internal static class StringExtensions
{
    public static string Sluggify(this string value) =>
        value
            .ToLowerInvariant()
            .Replace(' ', '-')
            .Replace('\'', '-');
}
