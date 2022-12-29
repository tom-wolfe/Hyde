namespace Hyde.Serializer;

class SiteSerializer : ISiteSerializer
{
    private readonly ILogger<SiteSerializer> _logger;
    private readonly SiteSerializerOptions _options;

    public SiteSerializer(ILogger<SiteSerializer> logger, IOptions<SiteSerializerOptions> options)
    {
        this._logger = logger;
        this._options = options.Value;
    }

    public async Task<SiteSerializeResult> Serialize(Site site, CancellationToken cancellationToken = default)
    {
        if (site.Root == null)
        {
            throw new InvalidOperationException("Site with null root.");
        }

        var stopwatch = Stopwatch.StartNew();
        this._logger.LogInformation("Serializing site to {dir}", this._options.OutputDirectory);
        if (this._options.Clean)
        {
            this._logger.LogInformation("Cleaning output directory");
            DirectoryUtils.Clean(this._options.OutputDirectory);
        }
        await SerializeDirectory(site.Root, this._options.OutputDirectory, false, cancellationToken);

        stopwatch.Stop();
        return new SiteSerializeResult() { Duration = stopwatch.Elapsed };
    }

    private static Task SerializeDirectory(SiteDirectory directory, string path, bool asSubDirectory, CancellationToken cancellationToken = default)
    {
        var currentDir = path;
        if (asSubDirectory)
        {
            currentDir = Path.Join(path, directory.Name);
            Directory.CreateDirectory(currentDir);
        }

        var dirTasks = directory.Directories.Select(d => SerializeDirectory(d, currentDir, true, cancellationToken));
        var fileTasks = directory.Files.Select(f => SerializeFile(f, currentDir, cancellationToken));

        return Task.WhenAll(dirTasks.Union(fileTasks));
    }

    private static async Task SerializeFile(SiteFile file, string path, CancellationToken cancellationToken = default)
    {
        var currentFile = Path.Join(path, file.Name);

        await using var readStream = file.GetContentStream();
        await using var writeStream = new FileStream(currentFile, FileMode.Create, FileAccess.Write);
        await readStream.CopyToAsync(writeStream, cancellationToken);
    }
}
