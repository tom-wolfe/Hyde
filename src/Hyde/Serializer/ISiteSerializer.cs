namespace Hyde.Serializer;

internal interface ISiteSerializer
{
    Task<SiteSerializeResult> Serialize(Site site, CancellationToken cancellationToken = default);
}
