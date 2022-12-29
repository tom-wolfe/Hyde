namespace Hyde.Mutator.Search;

internal class SearchIndexEntry
{
    public string Id { get; set; } = "";
    public string Title { get; set; } = "";
    public string Icon { get; set; } = "";
    public string? Description { get; set; }
    public string Content { get; set; } = "";
    public string Url { get; set; } = "";
}
