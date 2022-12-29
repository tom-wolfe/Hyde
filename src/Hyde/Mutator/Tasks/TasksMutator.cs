using System.Collections.Concurrent;
using System.Text.RegularExpressions;

namespace Hyde.Mutator.Tasks;

internal class TasksMutator : CollectorMutator<SiteFileTasks>
{
    private static readonly Regex TodoRegex = new(@"\[TODO:(?<task>.+)\]|TODO:(?<task>.+)");
    private static readonly Regex CheckboxRegex = new(@"^- \[ \] (?<task>.+)$");

    public TasksMutator(ILogger<TasksMutator> logger) : base(logger) { }

    protected override bool FileFilter(SiteFile file) => file.Extension == ".md";

    protected override async Task CollectFile(SiteFile file, ConcurrentBag<SiteFileTasks> collection, CancellationToken cancellationToken = default)
    {
        try
        {
            var contents = await file.GetContents();
            var fileTasks = GetTasks(contents);
            if (fileTasks.Any())
            {
                collection.Add(new SiteFileTasks(file, fileTasks));
            }
        }
        catch (Exception ex)
        {
            this.Logger.LogError(ex, "Error scanning {file}: {message}", file.GetRelativePath(), ex.Message);
        }
    }

    protected override Task PostCollection(Site site, ConcurrentBag<SiteFileTasks> collection, CancellationToken cancellationToken = default)
    {
        site.AddMetadata("task_pages", collection.ToList());
        return Task.CompletedTask;
    }

    private static List<string> GetTasks(string contents)
    {
        var result = new List<string>();

        var sr = new StringReader(contents);
        while (sr.ReadLine() is { } line)
        {
            var todoMatches = TodoRegex.Matches(line);
            for (var i = 0; i < todoMatches.Count; i++)
            {
                var match = todoMatches[i];
                result.Add(match.Groups["task"].Value);
            }

            var checkboxMatch = CheckboxRegex.Match(line);
            if (checkboxMatch.Success)
            {
                result.Add(checkboxMatch.Groups["task"].Value);
            }
        }

        return result;
    }
}
