using System.Text;

namespace Hyde.Domain;

internal class VirtualSiteFile : SiteFile
{
    private readonly string? _contents;

    public VirtualSiteFile(string name, string? contents = null) : base(name)
    {
        this._contents = contents;
    }

    protected override Stream GetOriginalContentStream() =>
        this._contents == null
            ? new MemoryStream()
            : new MemoryStream(Encoding.UTF8.GetBytes(this._contents));
}
