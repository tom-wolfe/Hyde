namespace Hyde.Mutator.Template.Layout;

internal interface ILayoutStore
{
    public ValueTask<SiteLayout> GetLayout(string template, CancellationToken cancellationToken = default);
}
