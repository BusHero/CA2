using System.Numerics;
using FluentAssertions;
using FsCheck;
using FsCheck.Xunit;

namespace CA2.Tests;

public class UnitTest1
{
    public static TheoryData<int[], int[], int> Data => new()
    {
        { [0, 0], [2, 2], 0 },
        { [0, 1], [2, 2], 1 },
        { [1, 0], [2, 2], 2 },
        { [1, 1], [2, 2], 3 },

        { [0, 0], [3, 2], 0 },
        { [0, 1], [3, 2], 1 },
        { [1, 0], [3, 2], 2 },
        { [1, 1], [3, 2], 3 },
        { [2, 0], [3, 2], 4 },
        { [2, 1], [3, 2], 5 },

        { [0, 0], [2, 3], 0 },
        { [0, 1], [2, 3], 1 },
        { [0, 2], [2, 3], 2 },
        { [1, 0], [2, 3], 3 },
        { [1, 1], [2, 3], 4 },
        { [1, 2], [2, 3], 5 },

        { [0, 0], [3, 3], 0 },
        { [0, 1], [3, 3], 1 },
        { [0, 2], [3, 3], 2 },
        { [1, 0], [3, 3], 3 },
        { [1, 1], [3, 3], 4 },
        { [1, 2], [3, 3], 5 },
        { [2, 0], [3, 3], 6 },
        { [2, 1], [3, 3], 7 },
        { [2, 2], [3, 3], 8 },

        { [0, 0, 0], [2, 2, 2], 0 },
        { [0, 0, 1], [2, 2, 2], 1 },
        { [0, 1, 0], [2, 2, 2], 2 },
        { [0, 1, 1], [2, 2, 2], 3 },
        { [1, 0, 0], [2, 2, 2], 4 },
        { [1, 0, 1], [2, 2, 2], 5 },
        { [1, 1, 0], [2, 2, 2], 6 },
        { [1, 1, 1], [2, 2, 2], 7 },
    };

    [Theory]
    [MemberData(nameof(Data))]
    public void GenerateNumber(
        int[] values,
        int[] sizes,
        int expectedResult)
    {
        var generator = new Generator();

        var number = Generator.Generate(values, sizes);

        number.Should().Be(expectedResult);
    }

    [Property(Arbitrary = [typeof(BigArraysOfZerosAndOnes)])]
    public Property ResultIsBiggerOrEqualToZero(
        int[] numbers)
    {
        var generator = new Generator();

        var sizes = GetSizes(numbers.Length);
        var number = Generator.Generate(numbers, sizes);

        return (0 <= number).ToProperty();
    }

    [Property(Arbitrary = [typeof(BigArraysOfZerosAndOnes)])]
    public Property ResultIsSmallerOrEqualToTheCount(
        int[] numbers)
    {
        var generator = new Generator();

        var sizes = GetSizes(numbers.Length);
        var number = Generator.Generate(numbers, sizes);

        return (number < BigInteger.Pow(2, numbers.Length)).ToProperty();
    }

    [Property(Arbitrary = [typeof(BigArraysOfZeros)])]
    public Property ArraysOfZerosHaveZero(int[] numbers)
    {
        var generator = new Generator();

        var sizes = GetSizes(numbers.Length);
        var number = Generator.Generate(numbers, sizes);

        return (number == 0).ToProperty();
    }

    [Property(Arbitrary = [typeof(BigArraysOfOnes)])]
    public Property ArraysOfOnesHaveMaxPower(int[] numbers)
    {
        var generator = new Generator();

        var sizes = GetSizes(numbers.Length);
        var number = Generator.Generate(numbers, sizes);

        return (number == BigInteger.Pow(2, numbers.Length) - 1).ToProperty();
    }

    private static int[] GetSizes(int numbersLength)
    {
        return Enumerable
            .Range(0, numbersLength)
            .Select(_ => 2)
            .ToArray();
    }
}

public class Combination
{
    public required int[] Item { get; init; }

    public required int[] Sizes { get; init; }
}

public interface IGenerator<T>
{
    static abstract Arbitrary<T> Generate();
}

public sealed class BigArraysOfOnes : IGenerator<int[]>
{
    private BigArraysOfOnes() { }
    
    public static Arbitrary<int[]> Generate()
    {
        return Gen
            .Elements(1)
            .ArrayOf()
            .ToArbitrary();
    }
}

public class BigArraysOfZeros : IGenerator<int[]>
{
    public static Arbitrary<int[]> Generate()
    {
        return Gen.Elements(0).ArrayOf().ToArbitrary();
    }
}

public class BigArraysOfZerosAndOnes : IGenerator<int[]>
{
    public static Arbitrary<int[]> Generate()
    {
        return Gen
            .Elements(0, 1)
            .ArrayOf()
            .ToArbitrary();
    }
}