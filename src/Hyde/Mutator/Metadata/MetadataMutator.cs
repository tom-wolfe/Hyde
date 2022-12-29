using Hyde.Services.Metadata;

namespace Hyde.Mutator.Metadata;

internal class MetadataMutator : FileMutator
{
    private readonly MetadataMutatorOptions _options;

    private readonly IMetadataExtractor _metadata;

    public MetadataMutator(ILogger<MetadataMutator> logger, IMetadataExtractor metadata, IOptions<MetadataMutatorOptions> options) : base(logger)
    {
        this._metadata = metadata;
        this._options = options.Value;
    }

    protected override bool FileFilter(SiteFile file) => file.Extension == ".md";

    protected override async Task MutateFile(Site site, SiteDirectory directory, SiteFile file, CancellationToken cancellationToken = default)
    {
        file.AddMetadata(this._options.Default);

        var contents = await file.GetContents();
        var result = await this._metadata.Extract(contents);
        file.AddMetadata(result.Metadata);
        file.SetContents(result.Remainder);
    }
}
