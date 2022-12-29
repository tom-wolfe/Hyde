using System.Collections.Concurrent;

namespace Hyde.Mutator.Template;

internal class LayoutPerformanceCache
{
    private readonly ConcurrentDictionary<string, ConcurrentBag<TimeSpan>> _layoutRuns = new(StringComparer.OrdinalIgnoreCase);

    public void RegisterTime(string layoutName, TimeSpan time)
    {
        var layout = this._layoutRuns.GetOrAdd(layoutName, _ => new ConcurrentBag<TimeSpan>());
        layout.Add(time);
    }

    public Dictionary<string, TimeSpan> GetAverageTimes()
    {
        return this._layoutRuns.ToDictionary(kvp => kvp.Key, kvp => kvp.Value.Average());
    }

    public Dictionary<string, TimeSpan> GetTotalTimes()
    {
        return this._layoutRuns.ToDictionary(kvp => kvp.Key, kvp => kvp.Value.Sum());
    }
}
