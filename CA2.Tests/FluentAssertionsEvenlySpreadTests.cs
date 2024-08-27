using AutoFixture.Xunit2;
using FluentAssertions;
using Xunit.Sdk;

namespace CA2.Tests;

public sealed class FluentAssertionsEvenlySpreadTests
{
    [Fact]
    public void TwoOnesAreNotEvenlySpread_FluentAssertions()
    {
        int[] range = [1, 1];

        var action = () => range
            .Should()
            .BeEvenlySpread(2, 0.05);

        action.Should()
            .Throw<XunitException>()
            .WithMessage("Expected collection to be evenly distributed, but found non-even distribution");
    }

    [Fact]
    public void NullCollectionIsNotEvenlySpread()
    {
        var range = default(int[]);

        var action = () => range
            .Should()
            .BeEvenlySpread(2, 0.05);

        action.Should()
            .Throw<XunitException>()
            .WithMessage("Expected collection to be evenly distributed, but found <null>");
    }

    [Theory, AutoData]
    public void ErrorMessageContainsBecause(string because)
    {
        int[] range = [1, 1];

        var action = () => range
            .Should()
            .BeEvenlySpread(2, 0.05, because);

        action.Should()
            .Throw<XunitException>()
            .WithMessage($"Expected collection to be evenly distributed {because}, but found non-even distribution");
    }

    [Theory, AutoData]
    public void ErrorMessageContainsArguments(string because)
    {
        int[] range = [1, 1];

        var action = () => range
            .Should()
            .BeEvenlySpread(2, 0.05, "{0}", because);

        action.Should()
            .Throw<XunitException>()
            .WithMessage($"Expected collection to be evenly distributed {because}, but found non-even distribution");
    }
    
    [Fact]
    public void ThePreviousAssertionIsReturned()
    {
        int[] range = [1, 0];

        var subject = range
            .Should()
            .BeEvenlySpread(2, 0.05)
            .And
            .Subject
            .Should()
            .BeEquivalentTo(range);
    }
}