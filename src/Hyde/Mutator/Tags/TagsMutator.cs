using System.Collections.Concurrent;

namespace Hyde.Mutator.Tags;

internal class TagsMutator : CollectorMutator<SiteFileTag>
{
    public TagsMutator(ILogger<TagsMutator> logger) : base(logger) { }

    protected override bool FileFilter(SiteFile file) => file.Extension == ".md";

    protected override async Task CollectFile(SiteFile file, ConcurrentBag<SiteFileTag> collection, CancellationToken cancellationToken = default)
    {
        try
        {
            var contents = await file.GetContents();
            file.Metadata.TryGetValue("tags", out var tags);
            if (tags is not IEnumerable<object> tagList)
            { return; }
            var stringTagList = tagList
                .Select(t => t.ToString())
                .Where(s => !string.IsNullOrWhiteSpace(s));
            foreach (var tag in stringTagList)
            {
                collection.Add(new SiteFileTag(file, tag!));
            }
        }
        catch (Exception ex)
        {
            this.Logger.LogError(ex, "Error reading {File}: {Message}", file.GetRelativePath(), ex.Message);
        }
    }

    protected override Task PostCollection(Site site, ConcurrentBag<SiteFileTag> collection, CancellationToken cancellationToken = default)
    {
        var filesByTag = collection
            .GroupBy(t => t.Tag)
            .Select(t => new SiteFilesByTag(t.Key, t.Select(x => x.File).ToList()))
            .ToList();

        site.AddMetadata("tags", filesByTag.ToList());
        return Task.CompletedTask;
    }
}
