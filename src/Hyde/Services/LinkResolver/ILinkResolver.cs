namespace Hyde.Services.LinkResolver;

internal interface ILinkResolver
{
    LinkResolutionResult? ResolveLink(Site site, string url, string? title, string? content);
}
