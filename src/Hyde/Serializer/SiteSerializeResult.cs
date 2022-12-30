namespace Hyde.Serializer;

internal class SiteSerializeResult
{
    public TimeSpan Duration { get; init; } = TimeSpan.Zero;

    public void Write(ILogger logger, int padding) => logger.LogInformation("{Label}: {Time}", "Serialize".PadRight(padding), this.Duration);
}
