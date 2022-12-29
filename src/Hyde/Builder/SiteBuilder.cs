using Hyde.Mutator;
using Hyde.Reader;
using Hyde.Serializer;

namespace Hyde.Builder;

internal class SiteBuilder : ISiteBuilder
{
    private readonly ISiteReader _reader;
    private readonly ISiteMutator _mutator;
    private readonly ISiteSerializer _serializer;

    public SiteBuilder(ISiteReader reader, ISiteMutator mutator, ISiteSerializer serializer)
    {
        this._reader = reader;
        this._mutator = mutator;
        this._serializer = serializer;
    }

    public async Task<SiteBuildResult> Build(CancellationToken cancellationToken = default)
    {
        var readResult = await this._reader.Read();
        var mutateResult = await this._mutator.Mutate(readResult.Site, cancellationToken);
        var serializeResult = await this._serializer.Serialize(readResult.Site, cancellationToken);

        return new SiteBuildResult(
            readResult,
            mutateResult,
            serializeResult
        );
    }
}
