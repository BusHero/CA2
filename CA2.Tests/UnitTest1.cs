using System.Numerics;
using FluentAssertions;
using FsCheck;
using FsCheck.Xunit;

namespace CA2.Tests;

public class UnitTest1
{
    public static TheoryData<int[], int> Data => new()
    {
        { [0, 0], 0 },
        { [0, 1], 1 },
        { [1, 0], 2 },
        { [1, 1], 3 },
        { [0, 0, 0], 0 },
        { [0, 0, 1], 1 },
        { [0, 1, 0], 2 },
        { [0, 1, 1], 3 },
        { [1, 0, 0], 4 },
        { [1, 0, 1], 5 },
        { [1, 1, 0], 6 },
        { [1, 1, 1], 7 },
    };

    [Theory]
    [MemberData(nameof(Data))]
    public void GenerateNumber(int[] values, int expectedResult)
    {
        var generator = new Generator();

        var number = generator.Generate(values);

        number.Should().Be(expectedResult);
    }

    [Property(Arbitrary = [typeof(BigArraysOfZerosAndOnes)])]
    public Property ResultIsBiggerOrEqualToZero(
        int[] numbers)
    {
        var generator = new Generator();

        var number = generator.Generate(numbers);

        return (0 <= number).ToProperty();
    }

    [Property(Arbitrary = [typeof(BigArraysOfZerosAndOnes)])]
    public Property ResultIsSmallerOrEqualToTheCount(
        int[] numbers)
    {
        var generator = new Generator();

        var number = generator.Generate(numbers);

        return (number < BigInteger.Pow(2, numbers.Length)).ToProperty();
    }

    [Property(Arbitrary = [typeof(BigArraysOfZeros)])]
    public Property ArraysOfZerosHaveZero(int[] numbers)
    {
        var generator = new Generator();

        var number = generator.Generate(numbers);

        return (number == 0).ToProperty();
    }

    [Property(Arbitrary = [typeof(BigArraysOfOnes)])]
    public Property ArraysOfOnesHaveMaxPower(int[] numbers)
    {
        var generator = new Generator();

        var number = generator.Generate(numbers);

        return (number == BigInteger.Pow(2, numbers.Length) - 1).ToProperty();
    }
}

public static class BigArraysOfOnes
{
    public static Arbitrary<int[]> Generate()
    {
        return Gen.Elements(1).ArrayOf().ToArbitrary();
    }
}

public static class BigArraysOfZeros
{
    public static Arbitrary<int[]> Generate()
    {
        return Gen.Elements(0).ArrayOf().ToArbitrary();
    }
}

public static class BigArraysOfZerosAndOnes
{
    public static Arbitrary<int[]> Generate()
    {
        return Gen
            .Elements(0, 1)
            .ArrayOf()
            .ToArbitrary();
    }
}

public sealed class Generator
{
    public BigInteger Generate(int[] values)
    {
        BigInteger result = 0;

        for (var i = 0; i < values.Length - 1; i++)
        {
            result *= 2;
            result += values[i] * 2;
        }

        if (values.Length != 0)
        {
            return result + values[^1];
        }

        return result;
    }
}
/*
 * Given I have the following
 *
 */