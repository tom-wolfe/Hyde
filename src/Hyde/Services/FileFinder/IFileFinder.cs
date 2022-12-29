namespace Hyde.Services.FileFinder;

internal interface IFileFinder
{
    IEnumerable<SiteFile> Search(Site site, params string?[] query);
    SiteFile? Find(Site site, params string?[] query);
    SiteFile? Find(Site site, Uri uri);
}
