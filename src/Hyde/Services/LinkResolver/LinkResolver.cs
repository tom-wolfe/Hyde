using Hyde.Services.FileFinder;

namespace Hyde.Services.LinkResolver;

internal class LinkResolver : ILinkResolver
{
    private readonly IFileFinder _finder;
    private readonly LinkResolverOptions _options;

    public LinkResolver(IOptions<LinkResolverOptions> options, IFileFinder finder)
    {
        this._options = options.Value;
        this._finder = finder;
    }

    public LinkResolutionResult? ResolveLink(Site site, string url, string? title, string? content)
    {
        var linkParts = url
            .Split(':', 2)
            .Select(x => x.Trim().ToLowerInvariant())
            .ToArray();
        var protocol = linkParts.ElementAtOrDefault(0) ?? "";
        var restOfLink = linkParts.ElementAtOrDefault(1);

        var text = restOfLink ?? title ?? content;
        if (text == null)
            return new LinkResolutionResult
            {
                Link = url,
                Class = null
            };

        if (!this._options.Protocols.TryGetValue(protocol, out var resolver))
        {
            return null;
        }

        switch (resolver.Type)
        {
            case ProtocolResolverType.Format:
                return new LinkResolutionResult
                {
                    Link = resolver.Format.Replace("{link}", text.Sluggify(), StringComparison.OrdinalIgnoreCase),
                    Class = resolver.Class
                };
            case ProtocolResolverType.Lookup:
                var match = this._finder.Find(site, url, title, content);
                if (match != null)
                {
                    return new LinkResolutionResult
                    {
                        Link = match.GetRelativePath(),
                        Class = resolver.Class
                    };
                }
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        return null;
    }
}
