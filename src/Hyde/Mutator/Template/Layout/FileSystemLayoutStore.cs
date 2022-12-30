using System.Collections.Concurrent;
using Hyde.Services.Metadata;

namespace Hyde.Mutator.Template.Layout;

internal class FileSystemLayoutStore : ILayoutStore
{
    private readonly IMetadataExtractor _metadata;

    private static readonly List<string> Extensions = new()
    {
        ".scriban-html",
        ".scriban-htm",
        ".sbn-html",
        ".sbn-htm",
        ".sbnhtml",
        ".sbnhtm",
        ".scriban-txt",
        ".sbn-txt",
        ".sbntxt",
        ".sbn",
        ".scriban",
        ".htm",
        ".html"
    };

    private readonly ConcurrentDictionary<string, SiteLayout> _layoutCache = new(StringComparer.OrdinalIgnoreCase);
    private readonly TemplateMutatorOptions _options;

    public FileSystemLayoutStore(IMetadataExtractor metadata, IOptions<TemplateMutatorOptions> options)
    {
        this._metadata = metadata;
        this._options = options.Value;
    }

    public async ValueTask<SiteLayout> GetLayout(string template, CancellationToken cancellationToken = default)
    {
        if (this._layoutCache.TryGetValue(template, out var cachedLayout))
        { return cachedLayout; }

        var layoutPath = this.FindLayoutFilePath(template);
        var layoutText = await File.ReadAllTextAsync(layoutPath, cancellationToken);

        var metadata = await this._metadata.Extract(layoutText);

        metadata.Metadata.TryGetValue("layout", out var parentLayout);
        var layout = new SiteLayout
        {
            ParentLayout = parentLayout?.ToString(),
            LayoutContent = metadata.Remainder
        };
        this._layoutCache.TryAdd(template, layout);

        return layout;
    }

    private string FindLayoutFilePath(string template)
    {
        var possibleNames = Extensions.Select(ext => template + ext).ToList();
        var possiblePaths = possibleNames.SelectMany(_ => this._options.IncludeDirectories, (file, directory) => Path.Join(directory, file));
        var match = possiblePaths.FirstOrDefault(File.Exists);
        if (match == null)
        {
            throw new InvalidOperationException("Template not found: " + template);
        }

        return match;
    }
}
