namespace Hyde.Domain;

[DebuggerDisplay("{Name,nq}")]
internal class SiteDirectory
{
    private readonly List<SiteFile> _files = new();
    private readonly List<SiteDirectory> _directories = new();

    public SiteDirectory(string name)
    {
        Name = name;
        Directories = _directories.AsReadOnly();
        Files = _files.AsReadOnly();
    }

    public IEnumerable<SiteDirectory> Directories { get; }
    public IEnumerable<SiteFile> Files { get; }
    public IEnumerable<SiteFile> FilesExcludingIndex => _files.Where(f => !f.IsIndex).ToList();
    public SiteFile? Index { get; private set; }
    public SiteDirectory? Parent { get; private set; }
    public string Name { get; }

    public void AddDirectory(SiteDirectory directory)
    {
        if (_directories.Contains(directory))
        { return; }
        _directories.Add(directory);
        directory.SetParent(this);
    }

    public void AddFile(SiteFile file)
    {
        if (_files.Contains(file))
        { return; }
        if (file.IsIndex)
        {
            Index = file;
        }

        _files.Add(file);
        file.SetParent(this);
    }

    public SiteDirectory FindOrCreateDirectory(string name)
    {
        name = PathUtils.StripLeadingAndTrailingSeparators(name);
        var firstSlash = PathUtils.IndexOfSeparator(name);
        var hasSlash = firstSlash != -1;
        var dirName = hasSlash ? name[..firstSlash] : name;

        var dir = Directories.FirstOrDefault(d => d.Name.Equals(dirName, StringComparison.OrdinalIgnoreCase));
        if (dir == null)
        {
            dir = new SiteDirectory(dirName);
            AddDirectory(dir);
        }

        if (hasSlash)
        {
            var restOfPath = name[(firstSlash + 1)..];
            return dir.FindOrCreateDirectory(restOfPath);
        }

        return dir;
    }

    public string GetRelativePath()
    {
        var parent = Parent?.GetRelativePath() ?? "/";
        return parent == "/" ? $"/{Name}" : $"{parent}/{Name}";
    }

    private void SetParent(SiteDirectory parent)
    {
        if (Parent == parent)
        { return; }
        Parent = parent;
        Parent.AddDirectory(this);
    }
}
