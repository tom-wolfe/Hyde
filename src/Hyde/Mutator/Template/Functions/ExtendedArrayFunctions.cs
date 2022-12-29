using System.Collections;
using System.ComponentModel;
using Scriban;
using Scriban.Parsing;
using Scriban.Runtime;

namespace Hyde.Mutator.Template.Functions;

internal static class ExtendedArrayFunctions
{
    public static IEnumerable Sort(TemplateContext context, SourceSpan span, object? list, string member) =>
        SortMany(context, span, list, member);

    public static IEnumerable SortMany(TemplateContext context, SourceSpan span, object? list, params string[] members)
    {
        if (list == null)
        {
            return Enumerable.Empty<object>();
        }

        if (list is not IEnumerable enumerable)
        {
            return new ScriptArray(1) { list };
        }

        var opList = enumerable.Cast<object>();
        if (!members.Any())
        {
            return opList.OrderBy(o => o.ToString(), NaturalStringComparer.Default);
        }

        var firstMember = members.First();
        var tailMembers = members.Skip(1).ToList();

        var firstSort = ParseFormatString(firstMember);
        var result = opList
            .OrderBy(v =>
            {
                var myPath = firstSort.MemberPath;
                return GetValueByPath(context, span, v, myPath);
            }, NaturalStringComparer.Default, firstSort.Direction);

        foreach (var member in tailMembers)
        {
            var thenSort = ParseFormatString(member);
            result = result.ThenBy(v =>
            {
                var myPath = thenSort.MemberPath;
                return GetValueByPath(context, span, v, myPath);
            }, NaturalStringComparer.Default, thenSort.Direction);
        }

        return result.ToList();
    }

    private static string? GetValueByPath(TemplateContext context, SourceSpan span, object instance, string memberPath)
    {
        var currentValue = instance;
        var members = memberPath.Split('.');
        foreach (var member in members)
        {
            var accessor = context.GetMemberAccessor(currentValue);
            if (!accessor.TryGetValue(context, span, currentValue, member, out var memberValue))
            {
                return "";
            }
            currentValue = memberValue;
        }
        return currentValue?.ToString() ?? "";
    }

    private static MemberSort ParseFormatString(string input)
    {
        var parts = input.Split(':', ' ');
        var path = parts[0];
        var direction = ListSortDirection.Ascending;
        if (parts.Length > 1)
        {
            var dir = parts[1];
            if (dir.Equals("d", StringComparison.OrdinalIgnoreCase))
            {
                direction = ListSortDirection.Descending;
            }
        }
        return new MemberSort(path, direction);
    }

    private record MemberSort(string MemberPath, ListSortDirection Direction);
}
