namespace Hyde.Mutator.Template;

internal class TemplateMutatorOptions
{
    public string DefaultTemplate { get; set; } = "default";
    public List<string> IncludeDirectories { get; } = new();
}
