namespace Hyde.Mutator.Styles;

internal class StylesMutatorOptions
{
    public string? Stylesheet { get; set; }
    public List<string> IncludeDirectories { get; } = new();
}
