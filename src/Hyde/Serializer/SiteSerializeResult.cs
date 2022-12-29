namespace Hyde.Serializer;

internal class SiteSerializeResult
{
    public TimeSpan Duration { get; init; } = TimeSpan.Zero;

    public void Log(ILogger logger, int padding)
    {
        logger.LogInformation("{label}: {time}", "Serialize".PadRight(padding), this.Duration);
    }
}
