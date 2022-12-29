namespace Hyde.Services.Metadata;
internal interface IMetadataExtractor
{
    Task<MetadataExtractionResult> Extract(string contents);
}
