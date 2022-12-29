namespace Hyde.Mutator.Metadata;

internal class MetadataMutatorOptions
{
    public Dictionary<string, object?> Default { get; } = new(StringComparer.OrdinalIgnoreCase);
}
