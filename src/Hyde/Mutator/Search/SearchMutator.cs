using System.Collections.Concurrent;
using System.Text.Json;
using HtmlAgilityPack;

namespace Hyde.Mutator.Search;

internal class SearchMutator : ISiteMutator
{
    private readonly ILogger<SearchMutator> _logger;

    private static readonly string[] IndexExtensions =
    {
        ".md",
        ".html"
    };

    public SearchMutator(ILogger<SearchMutator> logger)
    {
        this._logger = logger;
    }

    public async Task<ISiteMutateResult> Mutate(Site site, CancellationToken cancellationToken = default)
    {
        if (site.Root == null)
        {
            throw new InvalidOperationException("Site with null root.");
        }

        this._logger.LogInformation("Mutation Started");
        var stopwatch = Stopwatch.StartNew();

        var searchDir = site.Root.Directories.FirstOrDefault(d => d.Name.Equals("search"));

        if (searchDir != null)
        {
            var index = new ConcurrentBag<SearchIndexEntry>();
            await FillIndex(index, site.Root);
            var json = JsonSerializer.Serialize(index.ToList(), new JsonSerializerOptions(JsonSerializerDefaults.Web) { WriteIndented = true });

            var dataFile = new VirtualSiteFile("data.json", json);
            searchDir.AddFile(dataFile);
        }
        else
        {
            this._logger.LogWarning("No search directory found.");
        }

        stopwatch.Stop();
        this._logger.LogInformation("Mutation Complete");
        return new ScalarSiteMutateResult
        {
            Name = nameof(SearchMutator),
            Duration = stopwatch.Elapsed
        };
    }

    private Task FillIndex(ConcurrentBag<SearchIndexEntry> index, SiteDirectory directory)
    {
        var dirTasks = directory.Directories
            .Select(d => FillIndex(index, d));
        var filTasks = directory.Files
            .Where(f => IndexExtensions.Contains(f.Extension))
            .Select(f => FillIndex(index, f));

        return Task.WhenAll(dirTasks.Union(filTasks));
    }

    private async Task FillIndex(ConcurrentBag<SearchIndexEntry> index, SiteFile file)
    {
        try
        {
            var contents = await file.GetContents();
            var description = file.Metadata!.GetValueOrDefault("description", null)?.ToString();

            if (string.IsNullOrWhiteSpace(contents) && string.IsNullOrWhiteSpace(description))
            { return; }

            if (!file.Metadata.TryGetValue("title", out var title))
            {
                this._logger.LogError("{file} does not have a title", file.GetRelativePath());
                ;
            }

            if (!file.Metadata.TryGetValue("icon", out var icon))
            {
                this._logger.LogWarning("{file} does not have an icon", file.GetRelativePath());
                ;
            }

            if (file.Extension == ".html")
            {
                var document = new HtmlDocument();
                document.LoadHtml(contents);
                contents = document.DocumentNode.InnerText;
            }

            var entry = new SearchIndexEntry
            {
                Id = Guid.NewGuid().ToString(),
                Url = file.GetRelativePath(),
                Title = title?.ToString() ?? "",
                Description = description,
                Icon = icon?.ToString() ?? "",
                Content = contents
            };
            index.Add(entry);
        }
        catch (Exception ex)
        {
            this._logger.LogError(ex, "Error indexing {file}: {message}", file.GetRelativePath(), ex.Message);
        }
    }
}
