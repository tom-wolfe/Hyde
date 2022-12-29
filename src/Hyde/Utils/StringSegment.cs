namespace Hyde.Utils;

internal record StringSegment(string Value, StringSegmentType Type);

public enum StringSegmentType
{
    Text,
    Number,
}
