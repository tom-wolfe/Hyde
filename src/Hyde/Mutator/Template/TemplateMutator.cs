using Hyde.Mutator.Template.Layout;
using Scriban;
using Scriban.Runtime;
using Scriban.Syntax;

namespace Hyde.Mutator.Template;

internal class TemplateMutator : ISiteMutator
{
    private readonly ILogger<TemplateMutator> _logger;
    private readonly ILayoutStore _layoutStore;
    private readonly ITemplateContextFactory _contextFactory;
    private readonly TemplateMutatorOptions _options;

    public TemplateMutator(
        ILogger<TemplateMutator> logger,
        ILayoutStore layoutStore,
        ITemplateContextFactory contextFactory,
        IOptions<TemplateMutatorOptions> options
    )
    {
        this._logger = logger;
        this._layoutStore = layoutStore;
        this._contextFactory = contextFactory;
        this._options = options.Value;
    }

    private static bool FileFilter(SiteFile file) => file.Extension == ".html";

    public virtual async Task<ISiteMutateResult> Mutate(Site site, CancellationToken cancellationToken = default)
    {
        if (site.Root == null)
        {
            throw new InvalidOperationException("Site with null root.");
        }

        try
        {
            var stopwatch = Stopwatch.StartNew();
            this._logger.LogInformation("Mutation Started");

            var perfCache = new LayoutPerformanceCache();
            await this.MutateDirectory(site, site.Root, perfCache, cancellationToken);
            this._logger.LogInformation("Mutation Complete");

            var results = perfCache.GetAverageTimes()
                .OrderByDescending(kvp => kvp.Value)
                .Select(kvp => new ScalarSiteMutateResult { Name = kvp.Key, Duration = kvp.Value })
                .Cast<ISiteMutateResult>()
                .ToList();

            stopwatch.Stop();
            return new TemplateSiteMutateResult
            {
                Name = nameof(TemplateMutator),
                Results = results,
                Duration = stopwatch.Elapsed
            };
        }
        catch (Exception ex)
        {
            this._logger.LogCritical(ex, "Mutation Error");
            throw;
        }
    }

    private Task MutateDirectory(Site site, SiteDirectory directory, LayoutPerformanceCache perfCache, CancellationToken cancellationToken = default)
    {
        var dirTasks = directory.Directories.Select(subDir => this.MutateDirectory(site, subDir, perfCache, cancellationToken));
        var fileTasks = directory.Files.Where(FileFilter).Select(file => this.MutateFile(site, directory, file, perfCache, cancellationToken));
        return Task.WhenAll(dirTasks.Union(fileTasks));
    }

    private async Task MutateFile(Site site, SiteDirectory directory, SiteFile file, LayoutPerformanceCache perfCache, CancellationToken cancellationToken = default)
    {
        try
        {
            var context = this._contextFactory.CreateContext(site, file);
            var output = await RenderFile(file, context);
            context.CurrentGlobal.SetValue("content", output, false);

            var layoutName = file.Metadata.GetValueOrDefault("layout", null)?.ToString() ?? this._options.DefaultTemplate;
            output = await this.RenderLayout(layoutName, context, perfCache, cancellationToken);

            file.SetContents(output);
        }
        catch (ScriptRuntimeException ex)
        {
            this._logger.LogError(ex, "Error rendering file {file}: {message}", file.GetRelativePath(), ex.InnerException?.Message ?? ex.Message);
        }
        catch (Exception ex)
        {
            this._logger.LogError(ex, "Error rendering file {file}: {message}", file.GetRelativePath(), ex.Message);
        }
    }

    private static async Task<string> RenderFile(SiteFile file, TemplateContext context)
    {
        var fileContents = await file.GetContents();
        var contentTemplate = Scriban.Template.Parse(fileContents);
        var output = await contentTemplate.RenderAsync(context);
        return output;
    }

    private async Task<string> RenderLayout(string layoutName, TemplateContext context, LayoutPerformanceCache perfCache, CancellationToken cancellationToken = default)
    {
        var layout = await this._layoutStore.GetLayout(layoutName, cancellationToken);

        var timer = Stopwatch.StartNew();
        var layoutTemplate = Scriban.Template.Parse(layout.LayoutContent);
        var output = await layoutTemplate.RenderAsync(context);
        timer.Stop();
        perfCache.RegisterTime(layoutName, timer.Elapsed);

        if (layout.ParentLayout != null)
        {
            context.CurrentGlobal.SetValue("content", output, false);
            output = await this.RenderLayout(layout.ParentLayout, context, perfCache, cancellationToken);
        }

        return output;
    }
}
