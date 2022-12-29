using Scriban;

namespace Hyde.Mutator.Template;

internal interface ITemplateContextFactory
{
    TemplateContext CreateContext(Site site, SiteFile file);
}
