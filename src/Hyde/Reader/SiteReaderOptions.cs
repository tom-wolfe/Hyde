namespace Hyde.Reader;

internal class SiteReaderOptions
{
    public string SourceDirectory { get; set; } = "";
    public Dictionary<string, object?> Metadata { get; } = new(StringComparer.OrdinalIgnoreCase);
}
