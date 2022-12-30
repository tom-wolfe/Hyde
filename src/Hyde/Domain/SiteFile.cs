using System.Collections.ObjectModel;
using System.Text;

namespace Hyde.Domain;

[DebuggerDisplay("{Name,nq}")]
internal abstract class SiteFile : IComparable, IComparable<SiteFile>
{
    private readonly Dictionary<string, object?> _metadata = new(StringComparer.OrdinalIgnoreCase);

    private string? _contents;

    protected SiteFile(string name)
    {
        this.Name = name;
    }

    public string Extension => Path.GetExtension(this.Name);
    public IReadOnlyDictionary<string, object?> Metadata => new ReadOnlyDictionary<string, object?>(this._metadata);
    public string Name { get; private set; }
    public SiteDirectory? Parent { get; private set; }
    public bool IsIndex => this.Name.StartsWith("index.", StringComparison.OrdinalIgnoreCase);

    public void AddMetadata(IReadOnlyDictionary<string, object?> metadata, bool overwrite = true)
    {
        foreach (var (key, value) in metadata)
        {
            if (this._metadata.ContainsKey(key))
            {
                if (overwrite)
                {
                    this._metadata[key] = value;
                }
            }
            else
            {
                this._metadata.Add(key, value);
            }
        }
    }

    public async Task<string> GetContents()
    {
        if (this._contents == null)
        {
            await using var stream = this.GetOriginalContentStream();
            using var reader = new StreamReader(stream);
            this._contents = await reader.ReadToEndAsync();
        }
        return this._contents;
    }

    public Stream GetContentStream()
    {
        if (this._contents != null)
        {
            return new MemoryStream(Encoding.UTF8.GetBytes(this._contents));
        }
        return this.GetOriginalContentStream();
    }

    public string GetRelativePath()
    {
        var parent = this.Parent?.GetRelativePath() ?? "/";
        var name = this.IsIndex ? "" : this.Name;
        return parent == "/" ? $"/{name}" : $"{parent}/{name}";
    }

    public void Rename(string newName) =>
        // TODO: Validation like duplication, etc.
        this.Name = newName;

    public void SetContents(string? contents, string? newExtension = null)
    {
        this._contents = contents;
        if (newExtension != null)
        {
            this.ChangeExtension(newExtension);
        }
    }

    public void SetParent(SiteDirectory parent)
    {
        if (this.Parent == parent)
        { return; }

        this.Parent = parent;
        this.Parent.AddFile(this);
    }

    protected abstract Stream GetOriginalContentStream();

    private void ChangeExtension(string extension) => this.Name = Path.GetFileNameWithoutExtension(this.Name) + extension.ToLowerInvariant();

    public int CompareTo(SiteFile? other) => 0;

    public int CompareTo(object? obj) => 0;
}
