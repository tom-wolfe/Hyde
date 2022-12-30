using System.Globalization;

namespace Hyde.Utils;

internal class StringSegmentComparer : IComparer<StringSegment?>
{
    private readonly StringComparer _stringComparer;

    public StringSegmentComparer(StringComparer comparer)
    {
        this._stringComparer = comparer;
    }

    public int Compare(StringSegment? left, StringSegment? right)
    {
        if (left == null)
        { return ComparerUtils.LeftBeforeRight; }
        if (right == null)
        { return ComparerUtils.RightBeforeLeft; }

        if (left.Type != StringSegmentType.Number)
        {
            return right.Type == StringSegmentType.Number
                ? ComparerUtils.RightBeforeLeft
                : this._stringComparer.Compare(left.Value, right.Value);
        }

        if (right.Type != StringSegmentType.Number)
        {
            return ComparerUtils.LeftBeforeRight;
        }

        var lNum = int.Parse(left.Value, NumberStyles.Any, CultureInfo.InvariantCulture);
        var rNum = int.Parse(right.Value, CultureInfo.InvariantCulture);

        if (lNum == rNum)
        {
            return ComparerUtils.BothEqual;
        }

        return lNum < rNum ? ComparerUtils.LeftBeforeRight : ComparerUtils.RightBeforeLeft;
    }
}
