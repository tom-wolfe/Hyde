using System.Collections.Concurrent;

namespace Hyde.Mutator;

internal abstract class CollectorMutator<T> : ISiteMutator
{
    public CollectorMutator(ILogger<CollectorMutator<T>> logger)
    {
        this.Logger = logger;
    }

    protected ILogger<CollectorMutator<T>> Logger { get; }

    public async Task<ISiteMutateResult> Mutate(Site site, CancellationToken cancellationToken = default)
    {
        if (site.Root == null)
        {
            throw new InvalidOperationException("Site with null root.");
        }

        this.Logger.LogInformation("Mutation Started");
        var stopwatch = Stopwatch.StartNew();

        var collections = new ConcurrentBag<T>();
        await this.CollectDirectory(site, site.Root, collections, cancellationToken);

        await this.PostCollection(site, collections, cancellationToken);

        stopwatch.Stop();
        this.Logger.LogInformation("Mutation Complete");
        return new ScalarSiteMutateResult { Name = this.GetType().Name, Duration = stopwatch.Elapsed };
    }

    protected virtual bool FileFilter(SiteFile file) => true;

    protected virtual Task CollectDirectory(Site site, SiteDirectory directory, ConcurrentBag<T> collection, CancellationToken cancellationToken = default)
    {
        var dirTasks = directory.Directories.Select(subDir => this.CollectDirectory(site, subDir, collection));
        var fileTasks = directory.Files.Where(this.FileFilter).Select(file => this.CollectFile(file, collection, cancellationToken));
        return Task.WhenAll(dirTasks.Union(fileTasks));
    }

    protected abstract Task CollectFile(SiteFile file, ConcurrentBag<T> collection, CancellationToken cancellationToken = default);

    protected abstract Task PostCollection(Site site, ConcurrentBag<T> collection, CancellationToken cancellationToken = default);
}
