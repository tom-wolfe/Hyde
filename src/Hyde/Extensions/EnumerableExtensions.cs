using System.ComponentModel;

namespace Hyde.Extensions;

internal static class EnumerableExtensions
{
    public static TimeSpan Sum(this IEnumerable<TimeSpan> source) => source.Aggregate(TimeSpan.Zero, (current, i) => current + i);

    public static TimeSpan Average(this IEnumerable<TimeSpan> source)
    {
        var list = source.ToList();
        return TimeSpan.FromTicks(list.Ticks() / list.Count);
    }

    public static long Ticks(this IEnumerable<TimeSpan> source) => source.Aggregate(0L, (current, i) => current + i.Ticks);

    public static IOrderedEnumerable<TSource> OrderBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, IComparer<TKey>? comparer, ListSortDirection direction) => direction == ListSortDirection.Ascending ? source.OrderBy(keySelector, comparer) : source.OrderByDescending(keySelector, comparer);

    public static IOrderedEnumerable<TSource> ThenBy<TSource, TKey>(this IOrderedEnumerable<TSource> source, Func<TSource, TKey> keySelector, IComparer<TKey>? comparer, ListSortDirection direction) => direction == ListSortDirection.Ascending ? source.ThenBy(keySelector, comparer) : source.ThenByDescending(keySelector, comparer);
}
