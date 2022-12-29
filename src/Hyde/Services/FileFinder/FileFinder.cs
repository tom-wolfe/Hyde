namespace Hyde.Services.FileFinder;

internal class FileFinder : IFileFinder
{
    public SiteFile? Find(Site site, params string?[] query) => Search(site, query).FirstOrDefault();
    public SiteFile? Find(Site site, Uri uri)
    {
        if (site.Root == null)
        {
            throw new InvalidOperationException("Site with null root.");
        }
        return this.Search(site.Root, uri).FirstOrDefault();
    }

    public IEnumerable<SiteFile> Search(Site site, params string?[] query)
    {
        if (site.Root == null)
        {
            throw new InvalidOperationException("Site with null root.");
        }
        return Search(site.Root, query);
    }

    private IEnumerable<SiteFile> Search(SiteDirectory directory, params string?[] query)
    {
        bool FindByTitle(SiteFile file)
        {
            if (!file.Metadata.TryGetValue("title", out var title))
            {
                return false;
            }
            return query.Any(q => q?.Equals(title?.ToString(), StringComparison.OrdinalIgnoreCase) ?? false);
        }
        return this.Search(directory, FindByTitle);
    }

    private IEnumerable<SiteFile> Search(SiteDirectory directory, Uri uri)
    {
        bool FindByTitle(SiteFile file)
        {
            if (file is not FileBasedSiteFile targetFile) { return false; }
            var targetUri = new Uri(targetFile.SourcePath, UriKind.Absolute);
            return targetUri.Equals(uri);
        }
        return this.Search(directory, FindByTitle);
    }

    private IEnumerable<SiteFile> Search(SiteDirectory directory, Func<SiteFile, bool> predicate)
    {
        foreach (var file in directory.Files)
        {
            if (predicate(file))
            {
                yield return file;
            }
        }

        foreach (var dir in directory.Directories)
        {
            foreach (var file in this.Search(dir, predicate))
            {
                yield return file;
            }
        }
    }
}
