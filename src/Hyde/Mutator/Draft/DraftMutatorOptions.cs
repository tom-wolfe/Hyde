namespace Hyde.Mutator.Draft;

internal class DraftMutator : FileMutator
{
    private readonly DraftMutatorOptions _options;

    public DraftMutator(ILogger<DraftMutator> logger, IOptions<DraftMutatorOptions> options) : base(logger)
    {
        _options = options.Value;
    }

    protected override bool FileFilter(SiteFile file) => file.Extension == ".md";

    protected override Task MutateFile(Site site, SiteDirectory directory, SiteFile file, CancellationToken cancellationToken = default)
    {
        if (file.Metadata.TryGetValue(_options.DraftField, out var isDraft))
        {
            if (Convert.ToBoolean(isDraft))
            {
                directory.RemoveFile(file);
            }
        }

        return Task.CompletedTask;
    }
}
