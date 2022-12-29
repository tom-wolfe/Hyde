using Markdig;

namespace Hyde.Mutator.Markdown;

internal class MarkdownMutator : FileMutator
{
    private readonly MarkdownPipeline _pipeline;

    public MarkdownMutator(ILogger<MarkdownMutator> logger) : base(logger)
    {
        this._pipeline = new MarkdownPipelineBuilder()
            .UseAdvancedExtensions()
            .UseBootstrap()
            .Build();
    }

    protected override bool FileFilter(SiteFile file) => file.Extension == ".md";

    protected override async Task MutateFile(Site site, SiteDirectory directory, SiteFile file, CancellationToken cancellationToken = default)
    {
        var contents = await file.GetContents();
        var document = Markdig.Markdown.Parse(contents, this._pipeline);
        var html = document.ToHtml(this._pipeline);
        file.SetContents(html, ".html");
    }
}
