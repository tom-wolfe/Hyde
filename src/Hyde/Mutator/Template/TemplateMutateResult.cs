namespace Hyde.Mutator.Template;

internal class TemplateSiteMutateResult : ISiteMutateResult
{
    public TimeSpan Duration { get; init; }
    public string Name { get; init; } = "";
    public List<ISiteMutateResult> Results { get; set; } = new();

    public void Write(ILogger logger, int padding, int indent = 0)
    {
        var prefix = new string('-', indent * 2);
        logger.LogInformation("{Name}: {Time}", (prefix + this.Name).PadRight(padding), this.Duration);
        foreach (var result in this.Results)
        {
            result.Write(logger, padding, indent + 1);
        }
    }
}
