using LibSassHost;

namespace Hyde.Mutator.Styles;

internal class StylesMutator : ISiteMutator
{
    private readonly ILogger<StylesMutator> _logger;
    private readonly StylesMutatorOptions _options;

    public StylesMutator(ILogger<StylesMutator> logger, IOptions<StylesMutatorOptions> options)
    {
        this._logger = logger;
        this._options = options.Value;
    }

    public async Task<ISiteMutateResult> Mutate(Site site, CancellationToken cancellationToken = default)
    {
        if (site.Root == null)
        {
            throw new InvalidOperationException("Site with null root.");
        }

        this._logger.LogDebug("Mutation Started");
        var stopwatch = Stopwatch.StartNew();

        if (this._options.Stylesheet == null)
        {
            this._logger.LogDebug("No stylesheet, skipping SASS mutator.");
        }
        else
        {
            var stylesDir = new SiteDirectory("styles");

            var styleFile = new FileBasedSiteFile(this._options.Stylesheet);
            var contents = await styleFile.GetContents();

            var sassOptions = new CompilationOptions
            {
                SourceMap = true,
                IncludePaths = this._options.IncludeDirectories,
                SourceComments = false,
            };
            var result = SassCompiler.Compile(contents, this._options.Stylesheet, null, null, sassOptions);
            styleFile.SetContents(result.CompiledContent, ".css");

            var styleMap = new VirtualSiteFile(Path.GetFileName(this._options.Stylesheet), "");
            styleMap.SetContents(result.SourceMap, ".css.map");

            site.Root.AddDirectory(stylesDir);
            stylesDir.AddFile(styleFile);
            stylesDir.AddFile(styleMap);
        }

        stopwatch.Stop();
        this._logger.LogDebug("Mutation Complete");
        return new ScalarSiteMutateResult { Name = nameof(StylesMutator), Duration = stopwatch.Elapsed };
    }
}
