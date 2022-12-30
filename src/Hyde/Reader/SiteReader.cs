namespace Hyde.Reader;

internal class SiteReader : ISiteReader
{
    private readonly SiteReaderOptions _options;

    public SiteReader(IOptions<SiteReaderOptions> options)
    {
        this._options = options.Value;
    }

    public Task<SiteReadResult> Read()
    {
        var stopwatch = Stopwatch.StartNew();
        var site = new Site(this._options.SourceDirectory);
        var root = ReadDirectory(new DirectoryInfo(this._options.SourceDirectory), "");
        site.SetRoot(root);

        site.AddMetadata(this._options.Metadata);

        stopwatch.Stop();
        return Task.FromResult(new SiteReadResult(site)
        {
            Duration = stopwatch.Elapsed
        });
    }

    private static SiteDirectory ReadDirectory(DirectoryInfo directoryEntry, string? overrideName = null)
    {
        var directory = new SiteDirectory(overrideName ?? directoryEntry.Name);
        foreach (var entry in directoryEntry.EnumerateFileSystemInfos())
        {
            if (entry.Attributes.HasFlag(FileAttributes.Directory))
            {
                var dir = ReadDirectory((DirectoryInfo)entry);
                directory.AddDirectory(dir);
            }
            else
            {
                var file = new FileBasedSiteFile(entry.FullName);
                directory.AddFile(file);
            }
        }
        return directory;
    }
}
