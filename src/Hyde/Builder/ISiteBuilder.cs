namespace Hyde.Builder;

internal interface ISiteBuilder
{
    Task<SiteBuildResult> Build(CancellationToken cancellationToken = default);
}
