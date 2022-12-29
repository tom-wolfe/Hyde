using Hyde.Mutator.Template.Functions;
using Hyde.Mutator.Template.Layout;
using Hyde.Services.FileFinder;
using Scriban;
using Scriban.Functions;
using Scriban.Runtime;

namespace Hyde.Mutator.Template;

internal class TemplateContextFactory : ITemplateContextFactory
{
    private readonly ILayoutStore _layoutStore;
    private readonly IFileFinder _finder;
    private readonly ICustomTemplateFunctions _customFunctions;

    public TemplateContextFactory(ILayoutStore layoutStore, IFileFinder finder, ICustomTemplateFunctions customFunctions)
    {
        this._layoutStore = layoutStore;
        this._finder = finder;
        this._customFunctions = customFunctions;
    }

    public TemplateContext CreateContext(Site site, SiteFile file)
    {
        var builtIn = this.CreateBuiltIn();
        var context = new TemplateContext(builtIn)
        {
            TemplateLoader = new StoreTemplateLoader(this._layoutStore)
        };
        context.PushGlobal(new ScriptObject
        {
            { "site", site },
            { "page", file },
            { "directory", file.Parent }
        });

        return context;
    }

    private ScriptObject CreateBuiltIn()
    {
        var builtIn = new BuiltinFunctions();

        var arrayFunctions = (ArrayFunctions)builtIn["array"];
        arrayFunctions.Import(typeof(ExtendedArrayFunctions));

        var methods = typeof(ICustomTemplateFunctions).GetMethods();
        foreach (var method in methods)
        {
            var x = DynamicCustomFunction.Create(this._customFunctions, method);
            var name = StandardMemberRenamer.Default(method);
            builtIn.Add(name, x);
        }

        builtIn.Add("find", new FindCustomFunction(this._finder));

        return builtIn;
    }
}
