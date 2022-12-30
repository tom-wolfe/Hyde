namespace Hyde.Domain;

[DebuggerDisplay("{Name,nq}")]
internal class SiteDirectory
{
    private readonly List<SiteFile> _files = new();
    private readonly List<SiteDirectory> _directories = new();

    public SiteDirectory(string name)
    {
        this.Name = name;
        this.Directories = this._directories.AsReadOnly();
        this.Files = this._files.AsReadOnly();
    }

    public IEnumerable<SiteDirectory> Directories { get; }
    public IEnumerable<SiteFile> Files { get; }
    public IEnumerable<SiteFile> FilesExcludingIndex => this._files.Where(f => !f.IsIndex).ToList();
    public SiteFile? Index { get; private set; }
    public SiteDirectory? Parent { get; private set; }
    public string Name { get; }

    public void AddDirectory(SiteDirectory directory)
    {
        if (this._directories.Contains(directory))
        { return; }

        this._directories.Add(directory);
        directory.SetParent(this);
    }

    public void AddFile(SiteFile file)
    {
        if (this._files.Contains(file))
        { return; }
        if (file.IsIndex)
        {
            this.Index = file;
        }

        this._files.Add(file);
        file.SetParent(this);
    }

    public void RemoveFile(SiteFile file) => this._files.Remove(file);

    public SiteDirectory FindOrCreateDirectory(string name)
    {
        name = PathUtils.StripLeadingAndTrailingSeparators(name);
        var firstSlash = PathUtils.IndexOfSeparator(name);
        var hasSlash = firstSlash != -1;
        var dirName = hasSlash ? name[..firstSlash] : name;

        var dir = this.Directories.FirstOrDefault(d => d.Name.Equals(dirName, StringComparison.OrdinalIgnoreCase));
        if (dir == null)
        {
            dir = new SiteDirectory(dirName);
            this.AddDirectory(dir);
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
        var parent = this.Parent?.GetRelativePath() ?? "/";
        return parent == "/" ? $"/{this.Name}" : $"{parent}/{this.Name}";
    }

    private void SetParent(SiteDirectory parent)
    {
        if (this.Parent == parent)
        { return; }

        this.Parent = parent;
        this.Parent.AddDirectory(this);
    }
}
