using System.Text;

namespace Hyde.Domain;

internal class VirtualSiteFile : SiteFile
{
    private readonly string? _contents;

    public VirtualSiteFile(string name, string? contents = null) : base(name)
    {
        _contents = contents;
    }

    protected override Stream GetOriginalContentStream()
    {
        return _contents == null
            ? new MemoryStream()
            : new MemoryStream(Encoding.UTF8.GetBytes(_contents));
    }
}
