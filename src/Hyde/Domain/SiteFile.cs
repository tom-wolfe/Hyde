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
        Name = name;
    }

    public string Extension => Path.GetExtension(Name);
    public IReadOnlyDictionary<string, object?> Metadata => new ReadOnlyDictionary<string, object?>(_metadata);
    public string Name { get; private set; }
    public SiteDirectory? Parent { get; private set; }
    public bool IsIndex => Name.StartsWith("index.", StringComparison.OrdinalIgnoreCase);

    public void AddMetadata(IReadOnlyDictionary<string, object?> metadata, bool overwrite = true)
    {
        foreach (var (key, value) in metadata)
        {
            if (_metadata.ContainsKey(key))
            {
                if (overwrite)
                {
                    _metadata[key] = value;
                }
            }
            else
            {
                _metadata.Add(key, value);
            }
        }
    }

    public async Task<string> GetContents()
    {
        if (_contents == null)
        {
            await using var stream = GetOriginalContentStream();
            using var reader = new StreamReader(stream);
            _contents = await reader.ReadToEndAsync();
        }
        return _contents;
    }

    public Stream GetContentStream()
    {
        if (_contents != null)
        {
            return new MemoryStream(Encoding.UTF8.GetBytes(_contents));
        }
        return GetOriginalContentStream();
    }

    public string GetRelativePath()
    {
        var parent = Parent?.GetRelativePath() ?? "/";
        var name = IsIndex ? "" : Name;
        return parent == "/" ? $"/{name}" : $"{parent}/{name}";
    }

    public void Rename(string newName)
    {
        // TODO: Validation like duplication, etc.
        Name = newName;
    }

    public void SetContents(string? contents, string? newExtension = null)
    {
        _contents = contents;
        if (newExtension != null)
        {
            ChangeExtension(newExtension);
        }
    }

    public void SetParent(SiteDirectory parent)
    {
        if (Parent == parent)
        { return; }
        Parent = parent;
        Parent.AddFile(this);
    }

    protected abstract Stream GetOriginalContentStream();

    private void ChangeExtension(string extension)
    {
        Name = Path.GetFileNameWithoutExtension(Name) + extension.ToLowerInvariant();
    }

    public int CompareTo(SiteFile? other)
    {
        return 0;
    }

    public int CompareTo(object? obj)
    {
        return 0;
    }
}
