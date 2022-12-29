namespace Hyde.Mutator;

internal interface ISiteMutateResult
{
    string Name { get; }
    public TimeSpan Duration { get; }
    void Log(ILogger logger, int padding, int indent = 0);
}

internal class AggregateSiteMutateResult : ISiteMutateResult
{
    public string Name { get; init; } = "";
    public List<ISiteMutateResult> Results { get; init; } = new();
    public TimeSpan Duration => this.Results.Select(r => r.Duration).Sum();

    public void Log(ILogger logger, int padding, int indent = 0)
    {
        var prefix = new string(' ', indent * 2);
        logger.LogInformation("{name}: {time}", (prefix + this.Name).PadRight(padding), this.Duration);
        foreach (var result in this.Results)
        {
            result.Log(logger, padding, indent + 1);
        }
    }
}

internal class ScalarSiteMutateResult : ISiteMutateResult
{
    public string Name { get; init; } = "";
    public TimeSpan Duration { get; init; } = TimeSpan.Zero;

    public void Log(ILogger logger, int padding, int indent = 0)
    {
        var prefix = new string('-', indent * 2);
        logger.LogInformation("{name}: {time}", (prefix + this.Name).PadRight(padding), this.Duration);
    }
}
