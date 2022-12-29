﻿namespace Hyde.Mutator;

internal abstract class FileMutator : ISiteMutator
{
    protected readonly ILogger Logger;

    protected FileMutator(ILogger logger)
    {
        this.Logger = logger;
    }

    public virtual async Task<ISiteMutateResult> Mutate(Site site, CancellationToken cancellationToken = default)
    {
        try
        {
            var stopwatch = Stopwatch.StartNew();
            this.Logger.LogInformation("Mutation Started");
            await this.MutateDirectory(site, site.Root, cancellationToken);
            this.Logger.LogInformation("Mutation Complete");

            stopwatch.Stop();
            return new ScalarSiteMutateResult { Name = this.GetType().Name, Duration = stopwatch.Elapsed };
        }
        catch (Exception ex)
        {
            this.Logger.LogCritical(ex, "Mutation Error");
            throw;
        }
    }

    protected virtual Task MutateDirectory(Site site, SiteDirectory? directory, CancellationToken cancellationToken = default)
    {
        if (directory == null) { return Task.CompletedTask; }
        var dirTasks = directory.Directories.Select(subDir => this.MutateDirectory(site, subDir, cancellationToken));
        var fileTasks = directory.Files.Where(this.FileFilter).Select(file => this.MutateFileSafe(site, directory, file, cancellationToken));
        return Task.WhenAll(dirTasks.Union(fileTasks));
    }

    protected virtual bool FileFilter(SiteFile file) => true;

    private async Task MutateFileSafe(Site site, SiteDirectory directory, SiteFile file, CancellationToken cancellationToken = default)
    {
        try
        {
            await this.MutateFile(site, directory, file, cancellationToken);
        }
        catch (Exception ex)
        {
            this.Logger.LogError(ex, "Error mutating {file}: {message}", file.GetRelativePath(), ex.Message);
        }
    }

    protected abstract Task MutateFile(Site site, SiteDirectory directory, SiteFile file, CancellationToken cancellationToken = default);
}