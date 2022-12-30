namespace Hyde.Mutator;

/// <summary>
/// Represents the result of a call to <see cref="ISiteMutator.Mutate"/>.
/// </summary>
internal interface ISiteMutateResult
{
    /// <summary>
    /// Gets the name of the <see cref="ISiteMutator"/> that this result is for.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Gets the length of time it took the mutator to execute.
    /// </summary>
    public TimeSpan Duration { get; }

    /// <summary>
    /// Writes the result to the given logger.
    /// </summary>
    /// <param name="logger">The logger to which this result will be written.</param>
    /// <param name="padding"></param>
    /// <param name="indent"></param>
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
        logger.LogInformation("{Name}: {Time}", (prefix + this.Name).PadRight(padding), this.Duration);
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
        logger.LogInformation("{Name}: {Time}", (prefix + this.Name).PadRight(padding), this.Duration);
    }
}
