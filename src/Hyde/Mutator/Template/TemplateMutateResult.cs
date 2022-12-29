namespace Hyde.Mutator.Template;

internal class TemplateSiteMutateResult : ISiteMutateResult
{
    public TimeSpan Duration { get; init; }
    public string Name { get; init; } = "";
    public List<ISiteMutateResult> Results { get; set; } = new();

    public void Log(ILogger logger, int padding, int indent = 0)
    {
        var prefix = new string('-', indent * 2);
        logger.LogInformation("{name}: {time}", (prefix + this.Name).PadRight(padding), this.Duration);
        foreach (var result in this.Results)
        {
            result.Log(logger, padding, indent + 1);
        }
    }
}
