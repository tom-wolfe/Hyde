using System.Collections.ObjectModel;

namespace Hyde.Services.Metadata;

internal class MetadataExtractionResult
{
    public MetadataExtractionResult(IDictionary<string, object?> metadata, string remainder)
    {
        this.Metadata = new ReadOnlyDictionary<string, object?>(metadata);
        this.Remainder = remainder;
    }

    public IReadOnlyDictionary<string, object?> Metadata { get; }
    public string Remainder { get; }
}
