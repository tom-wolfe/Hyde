using System.Collections.ObjectModel;

namespace Hyde.Domain;

internal class Site
{
    private readonly Dictionary<string, object?> _metadata = new(StringComparer.OrdinalIgnoreCase);

    public Site(string sourcePath)
    {
        SourcePath = sourcePath;
    }

    public IReadOnlyDictionary<string, object?> Metadata => new ReadOnlyDictionary<string, object?>(_metadata);
    public SiteDirectory? Root { get; private set; }
    public string SourcePath { get; }

    public void SetRoot(SiteDirectory root)
    {
        Root = root;
    }

    public void AddMetadata(string key, object? value)
    {
        _metadata.Add(key, value);
    }

    public void AddMetadata(Dictionary<string, object?> metadata)
    {
        foreach (var (key, value) in metadata)
            AddMetadata(key, value);
    }
}
