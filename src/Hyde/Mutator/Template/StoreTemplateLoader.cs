using Hyde.Mutator.Template.Layout;
using Scriban;
using Scriban.Parsing;
using Scriban.Runtime;

namespace Hyde.Mutator.Template;

internal class StoreTemplateLoader : ITemplateLoader
{
    private readonly ILayoutStore _store;
    public StoreTemplateLoader(ILayoutStore store) { this._store = store; }
    public string GetPath(TemplateContext context, SourceSpan callerSpan, string templateName) => templateName;
    public string Load(TemplateContext context, SourceSpan callerSpan, string templatePath) => throw new InvalidOperationException("Synchronous template loading not supported");
    public async ValueTask<string> LoadAsync(TemplateContext context, SourceSpan callerSpan, string templatePath)
    {
        var layout = await this._store.GetLayout(templatePath);
        return layout.LayoutContent;
    }
}
