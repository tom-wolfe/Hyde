using System.Collections.ObjectModel;

namespace Hyde.Domain;

internal class Site
{
    private readonly Dictionary<string, object?> _metadata = new(StringComparer.OrdinalIgnoreCase);

    public Site(string sourcePath)
    {
        this.SourcePath = sourcePath;
    }

    public IReadOnlyDictionary<string, object?> Metadata => new ReadOnlyDictionary<string, object?>(this._metadata);
    public SiteDirectory? Root { get; private set; }
    public string SourcePath { get; }

    public void SetRoot(SiteDirectory root) => this.Root = root;

    public void AddMetadata(string key, object? value) => this._metadata.Add(key, value);

    public void AddMetadata(Dictionary<string, object?> metadata)
    {
        foreach (var (key, value) in metadata)
        {
            this.AddMetadata(key, value);
        }
    }
}
