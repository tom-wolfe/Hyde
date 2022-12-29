namespace Hyde.Services.LinkResolver;

internal class ProtocolResolver
{
    public string Format { get; set; } = "";
    public string? Class { get; set; }

    public ProtocolResolverType Type { get; set; } = ProtocolResolverType.Format;
}
