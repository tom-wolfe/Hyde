using Hyde.Mutator;
using Hyde.Reader;
using Hyde.Serializer;

namespace Hyde.Builder;

internal class SiteBuildResult
{
    public SiteBuildResult(SiteReadResult read, ISiteMutateResult mutate, SiteSerializeResult serialize)
    {
        this.Read = read;
        this.Mutate = mutate;
        this.Serialize = serialize;
    }

    public SiteReadResult Read { get; }
    public ISiteMutateResult Mutate { get; }
    public SiteSerializeResult Serialize { get; }
    public TimeSpan Duration => this.Read.Duration + this.Mutate.Duration + this.Serialize.Duration;

    public void Log(ILogger logger, int padding)
    {
        this.Read.Log(logger, padding);
        this.Mutate.Log(logger, padding);
        this.Serialize.Log(logger, padding);
        logger.LogInformation("{Name}: {Time}", "Total".PadRight(padding), this.Duration);
    }
}
