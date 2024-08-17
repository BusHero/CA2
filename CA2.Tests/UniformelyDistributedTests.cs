using FluentAssertions;

namespace CA2.Tests;

public class UniformlyDistributedTests
{
    [Fact]
    public void TwoOnesAreNotUniformlyDistributed()
    {
        int[] range = [1, 1];
        var result = StatisticalTests
            .IsUniformlyDistributed(range);

        result.Should().BeFalse();
    }

    [Fact]
    public void ZeroAndOneIsUniformlyDistributed()
    {
        int[] range = [1, 0];
        var result = StatisticalTests
            .IsUniformlyDistributed(range);

        result.Should().BeTrue();
    }

    [Fact]
    public void FiftyZerosAndFiftyOnesAreRandom()
    {
        var enum1 = Enumerable.Range(0, 50).Select(_ => 0);
        var enum2 = Enumerable.Range(0, 50).Select(_ => 1);
        var range = enum2.Concat(enum1);
        var result = StatisticalTests
            .IsUniformlyDistributed(range);

        result.Should().BeTrue();
    }

    [Fact]
    public void FiftyZerosAndFiftyOnesAreRandom2()
    {
        var enum1 = Enumerable.Range(0, 50).Select(_ => 0);
        var enum2 = Enumerable.Range(0, 50).Select(_ => 1);
        var range = enum1.Concat(enum2);
        var result = StatisticalTests
            .IsUniformlyDistributed(range);

        result.Should().BeTrue();
    }

    [Fact]
    public void FiftyZerosAndOneIsNotUniformlyDistributed()
    {
        var enum1 = Enumerable.Range(0, 50).Select(_ => 0);
        var range = enum1.Append(1);
        var result = StatisticalTests
            .IsUniformlyDistributed(range);

        result
            .Should()
            .BeFalse();
    }

    [Fact]
    public void FiftyZerosAnd49OnesAreUniformelyDistributed()
    {
        var enum1 = Enumerable.Range(0, 50).Select(_ => 0);
        var enum2 = Enumerable.Range(0, 49).Select(_ => 1);
        var range = enum1.Concat(enum2);
        var result = StatisticalTests
            .IsUniformlyDistributed(range);

        result
            .Should()
            .BeTrue();
    }
}

public static class StatisticalTests
{
    public static bool IsUniformlyDistributed(
        IEnumerable<int> range)
    {
        var list = range.ToList();

        int[] array = [0, 0];

        foreach (var i in list)
        {
            array[i]++;
        }

        var distribution = 1.0 / 2;
        var intermediateResult = array
            .Select(x => (double)x)
            .Select(x => x  / list.Count)
            .ToList();
        
        return intermediateResult
            .All(x => Math.Abs(x - distribution) < 0.05);
    }
}