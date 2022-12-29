namespace Hyde.Utils;

internal static class PathUtils
{
    private static readonly char[] DirectorySeparators = { '/', '\\' };

    public static string Absolutify(string href, FileBasedSiteFile file)
    {
        if (Path.IsPathFullyQualified(href))
        { return href; }

        var sourceDir = Path.GetDirectoryName(file.SourcePath);
        var absoluteLink = Path.Join(sourceDir, href);
        return absoluteLink;
    }

    public static string StripLeadingAndTrailingSeparators(string path)
    {
        var result = path;
        foreach (var sep in DirectorySeparators)
        {
            if (result.StartsWith(sep))
            { result = result[1..]; }
            if (result.EndsWith(sep))
            { result = result[..1]; }
        }
        return result;
    }

    public static int IndexOfSeparator(string path)
    {
        return path.IndexOfAny(DirectorySeparators);
    }
}
