namespace Hyde.Tests.Unit.Utils;

public class NaturalStringComparerTests
{
    [Fact]
    public void GivenTwoSimpleTextStrings_WhenComparing_ThenReturnFirstAlphabetically()
    {
        // Arrange
        var left = "a";
        var right = "b";

        // Act
        var comparer = NaturalStringComparer.Default;
        var x = comparer.Compare(left, right);

        // Assert
        x.Should().Be(ComparerUtils.LeftBeforeRight);
    }

    [Fact]
    public void GivenTwoTextStringsThatDifferByCase_WhenComparing_ThenReturnBothEqual()
    {
        // Arrange
        var left = "a";
        var right = "A";

        // Act
        var comparer = NaturalStringComparer.Default;
        var x = comparer.Compare(left, right);

        // Assert
        x.Should().Be(ComparerUtils.BothEqual);
    }

    [Fact]
    public void GivenOneNumericStringAndOneTextString_WhenComparing_ThenTheNumberComesFirst()
    {
        // Arrange
        var left = "123";
        var right = "Abc";

        // Act
        var comparer = NaturalStringComparer.Default;
        var x = comparer.Compare(left, right);

        // Assert
        x.Should().Be(ComparerUtils.LeftBeforeRight);
    }

    [Fact]
    public void GivenTwoNumbers_WhenComparing_ThenTheSmallestNumberComesFirst()
    {
        // Arrange
        var left = "test123";
        var right = "test94";

        // Act
        var comparer = NaturalStringComparer.Default;
        var x = comparer.Compare(left, right);

        // Assert
        x.Should().Be(ComparerUtils.RightBeforeLeft);
    }
}
