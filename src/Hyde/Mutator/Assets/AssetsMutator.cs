namespace Hyde.Mutator.Assets;

internal class AssetsMutator : ISiteMutator
{
    private readonly ILogger<AssetsMutator> _logger;
    private readonly AssetsMutatorOptions _options;

    public AssetsMutator(ILogger<AssetsMutator> logger, IOptions<AssetsMutatorOptions> options)
    {
        this._logger = logger;
        this._options = options.Value;
    }

    public Task<ISiteMutateResult> Mutate(Site site, CancellationToken cancellationToken = default)
    {
        this._logger.LogDebug("Mutation Started");
        var stopwatch = Stopwatch.StartNew();

        foreach (var (source, target) in this._options.Assets)
        {
            var attributes = File.GetAttributes(source);
            if (attributes.HasFlag(FileAttributes.Directory))
            {
                CopyDirectory(site, source, target);
            }
            else
            {
                CopyFile(site, source, target);
            }
        }

        stopwatch.Stop();
        this._logger.LogDebug("Mutation Complete");

        return Task.FromResult((ISiteMutateResult)new ScalarSiteMutateResult
        {
            Name = nameof(AssetsMutator),
            Duration = stopwatch.Elapsed
        });
    }

    private static void CopyDirectory(Site site, string source, string target)
    {
        if (site.Root == null)
        {
            throw new InvalidOperationException("Site with null root.");
        }

        var targetDirectory = site.Root.FindOrCreateDirectory(target);
        foreach (var dir in Directory.GetDirectories(source))
        {
            var dirName = Path.GetFileName(dir);
            var newSource = Path.Join(source, dirName);
            var newTarget = Path.Join(target, dirName);
            CopyDirectory(site, newSource, newTarget);
        }

        foreach (var file in Directory.GetFiles(source))
        {
            CopyFile(file, targetDirectory);
        }
    }

    private static void CopyFile(Site site, string source, string target)
    {
        if (site.Root == null)
        {
            throw new InvalidOperationException("Site with null root.");
        }

        if (Path.HasExtension(target))
        {
            var directory = Path.GetDirectoryName(target);
            var newName = Path.GetFileName(target);
            var parent = directory == null ? site.Root : site.Root.FindOrCreateDirectory(directory);
            CopyFile(source, parent, newName);
        }
        else
        {
            // Create file in directory.
            var parent = site.Root.FindOrCreateDirectory(target);
            CopyFile(source, parent);
        }
    }

    private static void CopyFile(string source, SiteDirectory target, string? newName = null)
    {
        // Create file in directory.
        var file = new FileBasedSiteFile(source);
        target.AddFile(file);
        if (newName != null)
        {
            file.Rename(newName);
        }
    }
}
