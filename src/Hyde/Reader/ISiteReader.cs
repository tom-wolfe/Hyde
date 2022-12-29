namespace Hyde.Reader;

internal interface ISiteReader
{
    Task<SiteReadResult> Read();
}
