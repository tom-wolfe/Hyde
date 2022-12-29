namespace Hyde.Tests.Unit.Utils;

public class NaturalStringReaderTests
{
    [Fact]
    public void GivenSimpleTextString_WhenReadingSegment_ThenReturnWholeString()
    {
        // Arrange
        var value = "test";

        // Act
        var comparer = new NaturalStringReader(value);
        var output = comparer.ReadNextSegment();

        // Assert
        output.Should().NotBeNull();
        output!.Type.Should().Be(StringSegmentType.Text);
        output.Value.Should().Be("test");
    }

    [Fact]
    public void GivenSimpleNumericString_WhenReadingSegment_ThenReturnWholeNumber()
    {
        // Arrange
        var value = "12345";

        // Act
        var comparer = new NaturalStringReader(value);
        var output = comparer.ReadNextSegment();

        // Assert
        output.Should().NotBeNull();
        output!.Type.Should().Be(StringSegmentType.Number);
        output.Value.Should().Be("12345");
    }

    [Fact]
    public void GivenStringWithTextAndNumbers_WhenReadingSegment_ThenReturnInSegments()
    {
        // Arrange
        var value = "test34";

        // Act
        var comparer = new NaturalStringReader(value);
        var segment1 = comparer.ReadNextSegment();
        var segment2 = comparer.ReadNextSegment();

        // Assert
        segment1.Should().NotBeNull();
        segment1!.Type.Should().Be(StringSegmentType.Text);
        segment1.Value.Should().Be("test");
        segment2.Should().NotBeNull();
        segment2!.Type.Should().Be(StringSegmentType.Number);
        segment2.Value.Should().Be("34");
    }

    [Fact]
    public void GivenAnyString_WhenLastSegmentIsRead_ThenReturnNull()
    {
        // Arrange
        var value = "test34";

        // Act
        var comparer = new NaturalStringReader(value);
        comparer.ReadNextSegment();
        comparer.ReadNextSegment();
        var segment3 = comparer.ReadNextSegment();

        // Assert
        segment3.Should().BeNull();
    }
}
