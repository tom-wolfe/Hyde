using Hyde.Services.FileFinder;
using Scriban;
using Scriban.Runtime;
using Scriban.Syntax;

namespace Hyde.Mutator.Template.Functions;

internal class FindCustomFunction : IScriptCustomFunction
{
    private readonly IFileFinder _finder;

    private readonly List<ScriptParameterInfo> _parameters = new()
    {
        new(typeof(string), "search", "")
    };

    public FindCustomFunction(IFileFinder finder)
    {
        this._finder = finder;
    }

    public ScriptParameterInfo GetParameterInfo(int index) => this._parameters[index];

    public int RequiredParameterCount => this._parameters.Count;
    public int ParameterCount => this._parameters.Count;
    public ScriptVarParamKind VarParamKind => ScriptVarParamKind.Direct;
    public Type ReturnType => typeof(SiteFile);

    public object Invoke(TemplateContext context, ScriptNode callerContext, ScriptArray arguments, ScriptBlockStatement blockStatement)
    {
        context.CurrentGlobal.TryGetValue("site", out var siteObj);
        if (siteObj is not Site site) { return null!; }
        var query = arguments[0].ToString();
        return this._finder.Find(site, query)!;
    }

    public ValueTask<object> InvokeAsync(TemplateContext context, ScriptNode callerContext, ScriptArray arguments, ScriptBlockStatement blockStatement)
    {
        var result = this.Invoke(context, callerContext, arguments, blockStatement);
        return ValueTask.FromResult(result);
    }
}
