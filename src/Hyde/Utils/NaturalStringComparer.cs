namespace Hyde.Utils;

public class NaturalStringComparer : IComparer<string?>
{
    private readonly StringComparer _stringComparer;

    public static readonly IComparer<string?> Default = new NaturalStringComparer(StringComparer.CurrentCultureIgnoreCase);
    private readonly StringSegmentComparer _segmentComparer;

    private NaturalStringComparer(StringComparer stringComparer)
    {
        this._stringComparer = stringComparer;
        this._segmentComparer = new StringSegmentComparer(this._stringComparer);
    }

    public int Compare(string? x, string? y)
    {
        // Base cases
        if (x == null)
        {
            return ComparerUtils.LeftBeforeRight;
        }
        if (y == null)
        {
            return ComparerUtils.RightBeforeLeft;
        }
        if (this._stringComparer.Equals(x, y))
        {
            return ComparerUtils.BothEqual;
        }

        // Get into segment checking.
        var lReader = new NaturalStringReader(x ?? "");
        var rReader = new NaturalStringReader(y ?? "");

        while (true)
        {
            var lSegment = lReader.ReadNextSegment();
            var rSegment = rReader.ReadNextSegment();

            if (x == null)
            {
                return ComparerUtils.LeftBeforeRight;
            }

            if (y == null)
            {
                return ComparerUtils.RightBeforeLeft;
            }

            var comparison = this._segmentComparer.Compare(lSegment, rSegment);
            if (comparison != ComparerUtils.BothEqual)
            {
                return comparison;
            }
        }
    }
}
