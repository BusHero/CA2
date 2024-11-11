
namespace CustomFluentAssertions.Tests;

public sealed class UniformlyDistributedTests
{
    [Fact]
    public void TwoOnesAreNotEvenlySpread()
    {
        int[] range = [1, 1];

        var result = range
            .IsEvenlySpread(2, 0.05);

        result.Should().BeFalse();
    }

    [Fact]
    public void ZeroAndOneIsEvenlySpread()
    {
        int[] range = [1, 0];

        var result = range
            .IsEvenlySpread(2, 0.05);

        result.Should().BeTrue();
    }

    [Fact]
    public void FiftyZerosAndFiftyOnesAreEvenlySpread()
    {
        var enum1 = Enumerable.Repeat(0, 50);
        var enum2 = Enumerable.Repeat(1, 50);
        var range = enum2.Concat(enum1);

        var result = range
            .IsEvenlySpread(2, 0.05);

        result.Should().BeTrue();
    }

    [Fact]
    public void FiftyZerosAndFiftyOnesAreEvenlySpread2()
    {
        var enum1 = Enumerable.Repeat(0, 50);
        var enum2 = Enumerable.Repeat(1, 50);
        var range = enum1.Concat(enum2);

        var result = range
            .IsEvenlySpread(2, 0.05);

        result.Should().BeTrue();
    }

    [Fact]
    public void FiftyZerosAndOneIsNotEvenlySpread()
    {
        var enum1 = Enumerable.Repeat(0, 50);
        var range = enum1.Append(1);

        var result = range
            .IsEvenlySpread(2, 0.05);

        result
            .Should()
            .BeFalse();
    }

    [Fact]
    public void FiftyZerosAnd49OnesAreEvenlySpread()
    {
        var enum1 = Enumerable.Repeat(0, 50);
        var enum2 = Enumerable.Repeat(1, 49);
        var range = enum1.Concat(enum2);
        var result = range
            .IsEvenlySpread(2, 0.05);

        result
            .Should()
            .BeTrue();
    }

    [Fact]
    public void FiftyZeros100OnesAreNotEvenlySpread()
    {
        var enum1 = Enumerable.Repeat(0, 50);
        var enum2 = Enumerable.Repeat(1, 100);

        var range = enum1.Concat(enum2);

        var result = range
            .IsEvenlySpread(2, 0.05);

        result
            .Should()
            .BeFalse();
    }

    [Fact]
    public void FiftyZerosFiftyOnesFiftyTwosAreEvenlySpread()
    {
        var enum1 = Enumerable.Repeat(0, 100);
        var enum2 = Enumerable.Repeat(1, 100);
        var enum3 = Enumerable.Repeat(2, 100);

        var range = enum1.Concat(enum2).Concat(enum3);

        var result = range
            .IsEvenlySpread(3, 0.05);

        result
            .Should()
            .BeTrue();
    }

    [Fact]
    public void NonEvenlySpreadNumbersWithBigToleranceShouldPass()
    {
        var enum1 = Enumerable.Repeat(0, 10);
        var enum2 = Enumerable.Repeat(1, 100);

        var range = enum1.Concat(enum2).ToList();

        var result = range
            .IsEvenlySpread(2, tolerance: 0.5);

        result
            .Should()
            .BeTrue();
    }

    [Fact]
    public void FiftyZerosAndFiftyOnesAreEvenlySpread3()
    {
        var enum1 = Enumerable.Repeat("0", 50);
        var enum2 = Enumerable.Repeat("1", 50);
        var range = enum2.Concat(enum1);

        var result = range
            .IsEvenlySpread(2, 0.05);

        result.Should().BeTrue();
    }
}