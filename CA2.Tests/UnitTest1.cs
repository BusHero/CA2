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

    [Property(Arbitrary = [typeof(GenerateStuff)])]
    public Property AllElementsAreSmallerOrEqualThanSize((int, int[]) tuple)
    {
        return tuple
            .Item2
            .All(x => x < tuple.Item1)
            .ToProperty();
    }

    [Property(Arbitrary = [typeof(GenerateStuff)])]
    public Property AllElementsAreBiggerOrEqualToZero((int, int[]) tuple)
    {
        return tuple
            .Item2
            .All(x => 0 <= x)
            .ToProperty();
    }

    [Property]
    public Property SizeOfTheGeneratorComesFromTheSizedProperty(PositiveInt size)
    {
        var sample = Gen.Sample(size.Item, 1, GenerateStuff.Generate().Generator)[0];

        var expectedSize = size.Item <= 2 ? 2 : size.Item;

        return (sample.Item1 == expectedSize).ToProperty();
    }

    [Property]
    public Property SizeIsNeverZero()
    {
        var sample = Gen
            .Sample(0, 1, GenerateStuff.Generate().Generator);

        return sample
            .Select(x => x.Item1)
            .All(x => x == 2)
            .ToProperty();
    }

    [Property(StartSize = 3, EndSize = 10)]
    public Property ElementsInListAreUniformlyDistributed(PositiveInt size)
    {
        var sample = Gen.Sample(size.Item, 1, GenerateStuff.Generate().Generator)[0];

        var result = new int[size.Item];

        foreach (var i in sample.Item2)
        {
            result[i]++;
        }

        var probability = 100.0 / size.Item;

        return result
            .Select(x => x * 100.0 / 1_000_000)
            .All(x => Math.Abs(x - probability) < 0.5)
            .ToProperty();
    }

    private static int[] GetSizes(int numbersLength)
    {
        return Enumerable
            .Range(0, numbersLength)
            .Select(_ => 2)
            .ToArray();
    }
}

public class Generators
{
    public static Arbitrary<(int, int)> Generate()
    {
        return Gen
            .Sized(size => Gen
                .Choose(0, size < 2 ? 1 : size - 1)
                .Select(x => (x, size < 2 ? 2 : size)))
            .ToArbitrary();
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

public sealed class GenerateStuff : IGenerator<(int, int[])>
{
    public static Arbitrary<(int, int[])> Generate() => Gen
        .Sized(size =>
        {
            size = size < 2 ? 2 : size;
            return Gen
                .Choose(0, size - 1)
                .ArrayOf(1_000_000)
                .Select(arr => (size, arr));
        })
        .ToArbitrary();
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