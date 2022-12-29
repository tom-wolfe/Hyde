using HtmlAgilityPack;
using Hyde.Services.FileFinder;
using Hyde.Services.LinkResolver;

namespace Hyde.Mutator.Link;

internal class LinkMutator : FileMutator
{
    private readonly IFileFinder _finder;
    private readonly ILinkResolver _resolver;

    public LinkMutator(ILogger<LinkMutator> logger, IFileFinder finder, ILinkResolver resolver) : base(logger)
    {
        this._finder = finder;
        this._resolver = resolver;
    }

    protected override bool FileFilter(SiteFile file) => file.Extension == ".html";

    protected override async Task MutateFile(Site site, SiteDirectory directory, SiteFile file, CancellationToken cancellationToken = default)
    {
        var html = await file.GetContents();
        var document = new HtmlDocument();
        document.LoadHtml(html);

        var links = document.DocumentNode.SelectNodes(".//link|.//a");
        if (links != null)
        {
            foreach (var link in links)
            {
                var hrefAttribute = link.Attributes["href"];
                if (hrefAttribute == null) { continue; }

                var href = hrefAttribute.Value;

                // Skip links with protocol settings http://
                if (href.Contains("://"))
                {
                    this.Logger.LogDebug("Skipping link with protocol: {href}", href);
                    continue;
                }

                this.Logger.LogDebug("Scanning link: {href}", href);

                // See if it's any of the custom protocols like spell: or monster:
                var resolvedLink = this._resolver.ResolveLink(site, href, link.Attributes["title"]?.Value, link.InnerText);
                if (resolvedLink != null)
                {
                    hrefAttribute.Value = resolvedLink.Link;
                    if (resolvedLink.Class != null)
                    {
                        link.AddClass(resolvedLink.Class);
                    }
                    continue;
                }

                // See if it's a link to an existing site file.
                if (file is FileBasedSiteFile sourceFile)
                {
                    var absoluteLink = PathUtils.Absolutify(href, sourceFile);
                    var absoluteUri = new Uri(absoluteLink, UriKind.Absolute);
                    var linkedMatch = this._finder.Find(site, absoluteUri);
                    if (linkedMatch != null)
                    {
                        this.Logger.LogDebug("Matching file found: {match}", linkedMatch.Name);
                        hrefAttribute.Value = linkedMatch.GetRelativePath();
                    }
                }

                // Finally, attempt to search for a page with a matching title.
                var match = this._finder.Find(site, href, link.Attributes["title"]?.Value, link.InnerText);
                if (match != null)
                {
                    hrefAttribute.Value = match.GetRelativePath();
                    continue;
                }
            }

            var writer = new MemoryStream();
            document.Save(writer);
            var reader = new StreamReader(writer);
            writer.Position = 0;
            var contents = await reader.ReadToEndAsync();

            file.SetContents(contents);
        }
    }
}
