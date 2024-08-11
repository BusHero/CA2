using System.Dynamic;
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

    [Property(Arbitrary = [typeof(TupleGenerator)])]
    public Property LengthOfValuesAndLengthOfSizesIsEqual((int[], int[]) tuple)
    {
        return (tuple.Item1.Length == tuple.Item2.Length).ToProperty();
    }

    [Property(Arbitrary = [typeof(TupleGenerator)])]
    public Property ItemsAndSizesAreDifferentArrays((int[], int[]) tuple)
    {
        return (tuple.Item1 != tuple.Item2).ToProperty();
    }

    [Property(Arbitrary = [typeof(TupleGenerator)])]
    public Property SizesAreBiggerThan2((int[], int[]) tuple)
    {
        return tuple.Item2.All(x => 2 <= x).ToProperty();
    }

    [Property(Arbitrary = [typeof(TupleGenerator)])]
    public Property ValuesAreBiggerOrEqualTo0((int[], int[]) tuple)
    {
        return tuple.Item1.All(x => 0 <= x).ToProperty();
    }

    [Property(Arbitrary = [typeof(TupleGenerator)])]
    public Property ValuesAreSmallerThanSizes((int[], int[]) tuple)
    {
        return tuple
            .Item1
            .Zip(tuple.Item2)
            .All(t => t.First < t.Second)
            .ToProperty();
    }

    [Property]
    public Property Generator2()
    {
        var foo = Gen
            .Choose(0, 9)
            .ArrayOf(1_000_000)
            .ToArbitrary();
        
        return Prop.ForAll(foo, numbers =>
        {
            int[] result = [0, 0, 0, 0, 0, 0, 0, 0, 0, 0];
            foreach (var i in numbers)
            {
                result[i]++;
            }

            const double probability = 100.0 / 10.0;

            return result
                .Select(x => x * 100.0 / 1_000_000)
                .All(x => Math.Abs(x - probability) < 0.1);
        });
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

public sealed class TupleGenerator : IGenerator<(int[], int[])>
{
    public static Arbitrary<(int[], int[])> Generate() => Gen
        .Sized(size => Arb
            .Default
            .Int32()
            .Generator
            .Where(x => 2 <= x)
            .Select(x => (x - 1, x))
            .ArrayOf(size)
            .Select(x => (
                x.Select(y => y.Item1).ToArray(),
                x.Select(y => y.Item2).ToArray())))
        .ToArbitrary();
}

public sealed class BigArraysOfOnes : IGenerator<int[]>
{
    public static Arbitrary<int[]> Generate() => Gen
        .Sized(size => Gen
            .Elements(1)
            .ArrayOf(size))
        .ToArbitrary();
}

public class BigArraysOfZeros : IGenerator<int[]>
{
    public static Arbitrary<int[]> Generate() => Gen
        .Sized(size => Gen
            .Elements(0)
            .ArrayOf(size))
        .ToArbitrary();
}

public class BigArraysOfZerosAndOnes : IGenerator<int[]>
{
    public static Arbitrary<int[]> Generate() => Gen
        .Sized(size => Gen
            .Elements(0, 1)
            .ArrayOf(size))
        .ToArbitrary();
}