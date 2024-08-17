using FluentAssertions;
using FluentAssertions.Collections;
using FluentAssertions.Execution;

namespace CA2.Tests;

public class UniformlyDistributedTests
{
    [Fact]
    public void TwoOnesAreNotUniformlyDistributed()
    {
        int[] range = [1, 1];

        var result = range.IsUniformlyDistributed(2, 0.05);

        result.Should().BeFalse();
    }

    [Fact]
    public void ZeroAndOneIsUniformlyDistributed()
    {
        int[] range = [1, 0];
        
        var result = range
            .IsUniformlyDistributed(2, 0.05);

        result.Should().BeTrue();
    }

    [Fact]
    public void FiftyZerosAndFiftyOnesAreRandom()
    {
        var enum1 = Enumerable.Range(0, 50).Select(_ => 0);
        var enum2 = Enumerable.Range(0, 50).Select(_ => 1);
        var range = enum2.Concat(enum1);
        
        var result = range
            .IsUniformlyDistributed(2, 0.05);

        result.Should().BeTrue();
    }

    [Fact]
    public void FiftyZerosAndFiftyOnesAreRandom2()
    {
        var enum1 = Enumerable.Range(0, 50).Select(_ => 0);
        var enum2 = Enumerable.Range(0, 50).Select(_ => 1);
        var range = enum1.Concat(enum2);
        
        var result = range
            .IsUniformlyDistributed(2, 0.05);

        result.Should().BeTrue();
    }

    [Fact]
    public void FiftyZerosAndOneIsNotUniformlyDistributed()
    {
        var enum1 = Enumerable.Range(0, 50).Select(_ => 0);
        var range = enum1.Append(1);
        
        var result = range
            .IsUniformlyDistributed(2, 0.05);

        result
            .Should()
            .BeFalse();
    }

    [Fact]
    public void FiftyZerosAnd49OnesAreUniformlyDistributed()
    {
        var enum1 = Enumerable.Range(0, 50).Select(_ => 0);
        var enum2 = Enumerable.Range(0, 49).Select(_ => 1);
        var range = enum1.Concat(enum2);
        var result = range
            .IsUniformlyDistributed(2, 0.05);

        result
            .Should()
            .BeTrue();
    }

    [Fact]
    public void FiftyZeros100OnesAreNotUniformlyDistributed()
    {
        var enum1 = Enumerable.Range(0, 50).Select(_ => 0);
        var enum2 = Enumerable.Range(0, 100).Select(_ => 1);
        
        var range = enum1.Concat(enum2);
        
        var result = range
            .IsUniformlyDistributed(2, 0.05);

        result
            .Should()
            .BeFalse();
    }
    
    [Fact]
    public void FiftyZerosFiftyOnesFiftyTwosAreUniforemlyDistributed()
    {
        var enum1 = Enumerable.Range(0, 100).Select(_ => 0);
        var enum2 = Enumerable.Range(0, 100).Select(_ => 1);
        var enum3 = Enumerable.Range(0, 100).Select(_ => 2);
        
        var range = enum1.Concat(enum2).Concat(enum3);
        
        var result = range
            .IsUniformlyDistributed(3, 0.05);

        result
            .Should()
            .BeTrue();
    }
    
    [Fact]
    public void NonUniformNumbersWithBigToleranceShouldPass()
    {
        var enum1 = Enumerable.Range(0, 10).Select(_ => 0);
        var enum2 = Enumerable.Range(0, 100).Select(_ => 1);
        
        var range = enum1.Concat(enum2).ToList();
        
        var result = range
            .IsUniformlyDistributed(2, tolerance: 0.5);

        range.Should().BeEvenlySpread(numberOfElements: 2, tolerance: 0.5);
        
        result
            .Should()
            .BeTrue();
    }
}

internal static class StatisticalTests
{

    public static AndConstraint<GenericCollectionAssertions<int>> BeEvenlySpread(
        this GenericCollectionAssertions<int> foo, 
        int numberOfElements, 
        double tolerance,
        string because = "", 
        params object[] becauseArgs)
    {
        Execute.Assertion
            .BecauseOf(because, becauseArgs)
            .WithExpectation("Expected {context:collection} to be uniformly distributed{reason}, ")
            .Given(() => foo.Subject)
            .ForCondition(subject => subject is not null)
            .FailWith("but found <null>")
            .Then
            .ForCondition(x => x.IsUniformlyDistributed(numberOfElements, tolerance))
            .FailWith("but found {0}.", foo.Subject)
            .Then
            .ClearExpectation();
        
        return new AndConstraint<GenericCollectionAssertions<int>>(foo);
    }
    
    public static bool IsUniformlyDistributed(
        this IEnumerable<int> range,
        int numberOfElements, 
        double tolerance)
    {
        var count = 0;

        var array = new int[numberOfElements];

        foreach (var i in range)
        {
            array[i]++;
            count++;
        }

        var distribution = 1.0 / numberOfElements;
        var intermediateResult = array
            .Select(x => (double)x)
            .Select(x => x  / count)
            .ToList();
        
        return intermediateResult
            .All(x => Math.Abs(x - distribution) < tolerance);
    }
}