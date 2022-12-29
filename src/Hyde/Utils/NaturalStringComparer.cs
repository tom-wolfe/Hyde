namespace Hyde.Utils;

public class NaturalStringComparer : IComparer<string?>
{
    private readonly StringComparer _stringComparer;

    public static readonly IComparer<string?> Default = new NaturalStringComparer(StringComparer.CurrentCultureIgnoreCase);
    private readonly StringSegmentComparer _segmentComparer;

    public NaturalStringComparer(StringComparer stringComparer)
    {
        this._stringComparer = stringComparer;
        this._segmentComparer = new StringSegmentComparer(this._stringComparer);
    }

    public int Compare(string? left, string? right)
    {
        // Base cases
        if (left == null)
        { return ComparerUtils.LeftBeforeRight; }
        if (right == null)
        { return ComparerUtils.RightBeforeLeft; }
        if (this._stringComparer.Equals(left, right))
        {
            return ComparerUtils.BothEqual;
        }

        // Get into segment checking.
        var lReader = new NaturalStringReader(left ?? "");
        var rReader = new NaturalStringReader(right ?? "");

        while (true)
        {
            var lSegment = lReader.ReadNextSegment();
            var rSegment = rReader.ReadNextSegment();

            if (left == null)
            { return ComparerUtils.LeftBeforeRight; }
            if (right == null)
            { return ComparerUtils.RightBeforeLeft; }

            var comparison = this._segmentComparer.Compare(lSegment, rSegment);
            if (comparison != ComparerUtils.BothEqual)
            { return comparison; }
        }
    }
}
