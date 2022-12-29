namespace Hyde.Services.LinkResolver;

internal class ProtocolResolverCollection : Dictionary<string, ProtocolResolver>
{
    public ProtocolResolverCollection() : base(StringComparer.OrdinalIgnoreCase)
    {

    }
}
