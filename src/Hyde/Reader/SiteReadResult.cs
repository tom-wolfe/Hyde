namespace Hyde.Reader;

internal class SiteReadResult
{
    public SiteReadResult(Site site)
    {
        this.Site = site;
    }

    public Site Site { get; }

    public TimeSpan Duration { get; init; } = TimeSpan.Zero;

    public void Write(ILogger logger, int padding) => logger.LogInformation("{Label}: {Time}", "Read".PadRight(padding), this.Duration);
}
