namespace Hyde.Mutator.Template.Functions;

internal class CustomTemplateFunctions : ICustomTemplateFunctions
{
    public string Link(SiteFile page) => page.GetRelativePath();

    public List<SiteFile> Breadcrumbs(SiteFile page)
    {
        var pages = new List<SiteFile> { page };
        var parentDir = page.Parent;
        while (parentDir != null)
        {
            if (parentDir.Index != null && parentDir.Index != page)
            {
                pages.Add(parentDir.Index!);
            }
            parentDir = parentDir.Parent;
        };

        // Remove the header.
        pages.RemoveAt(pages.Count - 1);
        pages.Reverse();
        return pages;
    }

    public string Sluggify(string input) => input.Sluggify();
}
