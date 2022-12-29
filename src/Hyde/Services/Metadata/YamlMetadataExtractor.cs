using System.Text;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace Hyde.Services.Metadata;

internal class YamlMetadataExtractor : IMetadataExtractor
{
    private const string YamlMarker = "---";

    private readonly IDeserializer _deserializer;

    public YamlMetadataExtractor()
    {
        this._deserializer = new DeserializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .WithAttemptingUnquotedStringTypeDeserialization()
            .Build();
    }

    public async Task<MetadataExtractionResult> Extract(string contents)
    {

        var reader = new StringReader(contents);

        var metadata = new Dictionary<string, object?>();
        if (contents.StartsWith(YamlMarker))
        {
            await reader.ReadLineAsync();
            var yaml = new StringBuilder();
            while (true)
            {
                var nextLine = await reader.ReadLineAsync();
                if (nextLine == YamlMarker) { break; }

                yaml.AppendLine(nextLine);
            }

            // Store it against the file.
            var yamlReader = new StringReader(yaml.ToString());

            metadata = this._deserializer.Deserialize<Dictionary<string, object?>>(yamlReader);

            // Attempt to coerce to date time because I can't get the YAML converter working.
            foreach (var kvp in metadata)
            {
                if (DateTime.TryParse(kvp.Value?.ToString(), out var date))
                {
                    metadata[kvp.Key] = date;
                }
            }
        }

        var remainder = await reader.ReadToEndAsync();

        return new MetadataExtractionResult(metadata, remainder);
    }
}
