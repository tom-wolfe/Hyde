using System.Collections.Concurrent;

namespace Hyde.Mutator;

/// <summary>
/// Provides functionality for aggregating values across all files and directories.
/// </summary>
/// <typeparam name="T">The type of value being aggregated.</typeparam>
internal abstract class CollectorMutator<T> : ISiteMutator
{
    protected CollectorMutator(ILogger<CollectorMutator<T>> logger)
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

        this.Logger.LogDebug("Mutation Started");
        var stopwatch = Stopwatch.StartNew();

        var collections = new ConcurrentBag<T>();
        await this.CollectDirectory(site, site.Root, collections, cancellationToken);

        await this.PostCollection(site, collections, cancellationToken);

        stopwatch.Stop();
        this.Logger.LogDebug("Mutation Complete");
        return new ScalarSiteMutateResult { Name = this.GetType().Name, Duration = stopwatch.Elapsed };
    }

    /// <summary>
    /// A method that can be used to filter which files are 'collected'.
    /// </summary>
    /// <param name="file">The file to check.</param>
    /// <returns>True if the file should be passed to the <see cref="PostCollection"/> method, otherwise false.</returns>
    protected virtual bool FileFilter(SiteFile file) => true;

    /// <summary>
    /// Aggregates and collects values across all files in a directory.
    /// </summary>
    /// <param name="site">The current site being collected.</param>
    /// <param name="directory">The current directory.</param>
    /// <param name="collection">The collection to which required values should be added.</param>
    /// <param name="cancellationToken">A token that can be used to cancel the operation.</param>
    protected virtual Task CollectDirectory(Site site, SiteDirectory directory, ConcurrentBag<T> collection, CancellationToken cancellationToken = default)
    {
        var dirTasks = directory.Directories.Select(subDir => this.CollectDirectory(site, subDir, collection));
        var fileTasks = directory.Files.Where(this.FileFilter).Select(file => this.CollectFile(file, collection, cancellationToken));
        return Task.WhenAll(dirTasks.Union(fileTasks));
    }

    /// <summary>
    /// Collects all required values from the given file.
    /// </summary>
    /// <param name="file">The file from which values should be collected.</param>
    /// <param name="collection">The collection to which required values should be added.</param>
    /// <param name="cancellationToken">A token that can be used to cancel the operation.</param>
    protected abstract Task CollectFile(SiteFile file, ConcurrentBag<T> collection, CancellationToken cancellationToken = default);

    /// <summary>
    /// The method called after the collection process is finished.
    /// </summary>
    /// <param name="site">The current site being collected.</param>
    /// <param name="collection">The values collected throughout the process.</param>
    /// <param name="cancellationToken">A token that can be used to cancel the operation.</param>
    protected abstract Task PostCollection(Site site, ConcurrentBag<T> collection, CancellationToken cancellationToken = default);
}
