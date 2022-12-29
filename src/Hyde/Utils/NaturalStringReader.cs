namespace Hyde.Utils;

internal class NaturalStringReader
{
    private const int EndOfString = -1;

    private readonly StringReader _reader;

    public NaturalStringReader(string input) : this(new StringReader(input)) { }

    public NaturalStringReader(StringReader reader)
    {
        this._reader = reader;
    }

    public StringSegment? ReadNextSegment()
    {
        var nextChar = this._reader.Peek();
        if (nextChar == EndOfString)
            return null;
        return IsNumber(nextChar)
            ? this.ReadNumericSegment()
            : this.ReadStringSegment();
    }

    private StringSegment ReadStringSegment() =>
        this.GetWhile(c => !IsNumber(c), StringSegmentType.Text);

    private StringSegment ReadNumericSegment() =>
        this.GetWhile(IsNumber, StringSegmentType.Number);

    private StringSegment GetWhile(Func<int, bool> predicate, StringSegmentType segmentType)
    {
        var r = "";
        do
        {
            r += (char)this._reader.Read();
        } while (this._reader.Peek() != EndOfString && predicate(this._reader.Peek()));
        return new StringSegment(r, segmentType);
    }

    private static bool IsNumber(int nextChar) => char.IsNumber((char)nextChar);
}
