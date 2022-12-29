namespace Hyde.Mutator.Template.Functions;

internal interface ICustomTemplateFunctions
{
    string Link(SiteFile page);
    List<SiteFile> Breadcrumbs(SiteFile page);
    string Sluggify(string input);
}
