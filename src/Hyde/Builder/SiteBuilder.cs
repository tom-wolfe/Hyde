using Hyde.Mutator;
using Hyde.Reader;
using Hyde.Serializer;

namespace Hyde.Builder;

internal class SiteBuilder : ISiteBuilder
{
    private readonly ILogger<SiteBuilder> _logger;
    private readonly ISiteReader _reader;
    private readonly ISiteMutator _mutator;
    private readonly ISiteSerializer _serializer;

    public SiteBuilder(ILogger<SiteBuilder> logger, ISiteReader reader, ISiteMutator mutator, ISiteSerializer serializer)
    {
        this._logger = logger;
        this._reader = reader;
        this._mutator = mutator;
        this._serializer = serializer;
    }

    public async Task<SiteBuildResult> Build(CancellationToken cancellationToken = default)
    {
        this._logger.LogInformation("Reading site");
        var readResult = await this._reader.Read();

        this._logger.LogInformation("Mutating site");
        var mutateResult = await this._mutator.Mutate(readResult.Site, cancellationToken);

        this._logger.LogInformation("Writing site");
        var serializeResult = await this._serializer.Serialize(readResult.Site, cancellationToken);

        return new SiteBuildResult(
            readResult,
            mutateResult,
            serializeResult
        );
    }
}
