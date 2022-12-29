namespace Hyde.Serializer;

interface ISiteSerializer
{
    Task<SiteSerializeResult> Serialize(Site site, CancellationToken cancellationToken = default);
}
