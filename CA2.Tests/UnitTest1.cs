using System.Numerics;
using System.Reflection;
using FluentAssertions;
using FsCheck;
using FsCheck.Xunit;
using Microsoft.FSharp.Core;
using Xunit.Sdk;

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
        { [1, 1, 0, 0], [2, 2, 2, 2], 12 },
        { [1, 2, 1, 1, 1, 0, 0], [2, 7, 3, 2, 2, 2, 2], 33_771 },
        { [1, 0, 0, 1, 0, 0, 1, 4, 1], [2, 2, 2, 2, 2, 2, 2, 7, 2], 321 }
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

    [Property]
    public Property ResultIsBiggerOrEqualToZero()
    {
        var arb = Gen
            .Sized(size => Gen
                .Elements(0, 1)
                .ArrayOf(size))
            .ToArbitrary();

        return Prop
            .ForAll(arb, numbers =>
            {
                var sizes = GetSizes(numbers.Length);

                var number = Generator.Generate(numbers, sizes);

                return (0 <= number).ToProperty();
            });
    }

    [Property]
    public Property ResultIsSmallerOrEqualToTheCount()
    {
        var arb = Gen
            .Sized(size => Gen
                .Elements(0, 1)
                .ArrayOf(size))
            .ToArbitrary();

        return Prop
            .ForAll(arb, numbers =>
            {
                var sizes = GetSizes(numbers.Length);

                var number = Generator.Generate(numbers, sizes);

                return (number < BigInteger.Pow(2, numbers.Length)).ToProperty();
            });
    }

    [Property]
    public Property ArraysOfZerosHaveZero()
    {
        var arb = Gen
            .Sized(size => Gen
                .Elements(0)
                .ArrayOf(size))
            .ToArbitrary();

        return Prop.ForAll(arb, numbers =>
        {
            var sizes = GetSizes(numbers.Length);
            var number = Generator.Generate(numbers, sizes);

            return (number == 0).ToProperty();
        });
    }

    [Property]
    public Property ArraysOfOnesHaveMaxPower()
    {
        var arb = Gen
            .Sized(size => Gen
                .Elements(1)
                .ArrayOf(size))
            .ToArbitrary();

        return Prop.ForAll(arb, numbers =>
        {
            var sizes = GetSizes(numbers.Length);
            var number = Generator.Generate(numbers, sizes);

            return (number == BigInteger.Pow(2, numbers.Length) - 1).ToProperty();
        });
    }

    [Property(Arbitrary = [typeof(Generators)])]
    public Property Foo(Combination combination)
    {
        var result = Generator.Generate(
            combination.Item,
            combination.Sizes);

        var foo = CalculateMaximumNumber(combination.Sizes);

        return (result <= foo).ToProperty().Label($"{result} is smaller than {foo}");
    }

    [Theory, MemberData(nameof(Sizes))]
    public void MaximumNumberIsCalculatedCorrectly(int[] sizes, BigInteger expectedResult)
    {
        var result = CalculateMaximumNumber(sizes);

        result.Should().Be(expectedResult);
    }

    public static TheoryData<int[], BigInteger> Sizes => new TheoryData<int[], BigInteger>()
    {
        { [2], 2 },
        { [3], 3 },
        { [10], 10 },
        { [10, 10], 100 },
        { [10, 10, 10], 1000 },
        { [2, 2, 2], 8 },
        { [2, 7, 3, 2, 2, 2, 2], 672 }
    };

    [Property]
    public Property LastItemIsEqualToOne(NonEmptyArray<PositiveInt> sizes)
    {
        var generatedSequence = CalculateSizes(sizes.Item.Select(x => x.Item).ToArray());

        var prop = new Func<bool>(() => generatedSequence[^1] == 1)
            .Label("Last item is 1");

        return prop;
    }

    [Property]
    public Property FirstItemIsEqualToTheLastOne()
    {
        var arb = Arb.Default.PositiveInt()
            .Generator
            .Select(x => x.Item)
            .ArrayOf(2)
            .ToArbitrary();

        return Prop.ForAll(arb, sizes =>
        {
            var generatedSequence = CalculateSizes(sizes);

            return (generatedSequence[0] == sizes[^1])
                .Label("Last item is equal to the last one");
        });
    }

    [Property]
    public Property GeneratedArrayIsArrangedDescending(NonEmptyArray<PositiveInt> sizes)
    {
        var generatedSequence = CalculateSizes(sizes.Item.Select(x => x.Item).ToArray());

        return generatedSequence
            .OrderDescending()
            .SequenceEqual(generatedSequence)
            .Label($"The generated array should be sorted [{string.Join(", ", generatedSequence)}]");
    }

    [Property]
    public Property SizeOfGeneratedArrayIsEqualToTheOriginalSize(NonEmptyArray<PositiveInt> sizes)
    {
        var generatedSequence = CalculateSizes(sizes.Item.Select(x => x.Item).ToArray());

        return (generatedSequence.Length == sizes.Item.Length)
            .Label("Size does matter");
    }

    [Property]
    public Property FirstItemIsTheProductOfPreviousNumbers(NonEmptyArray<PositiveInt> sizes)
    {
        var generatedSequence = CalculateSizes(sizes.Item.Select(x => x.Item).ToArray());

        return new Func<bool>(() =>
                generatedSequence[0] == sizes.Item.Skip(1)
                    .Select(x => x.Item)
                    .Select(x => (BigInteger)x)
                    .Aggregate(BigInteger.One, (fst, snd) => fst * snd))
            .When(2 <= sizes.Item.Length);
    }

    [Property(MaxTest = 10_000)]
    public Property ItemIsEqualToThePreviousGeneratedValueAndOriginalSize(NonEmptyArray<PositiveInt> sizes)
    {
        var generatedSequence = CalculateSizes(sizes.Item.Select(x => x.Item).ToArray());

        return new Func<bool>(() =>
            {
                return generatedSequence
                    .SkipLast(1)
                    .Select((x, i) => (x, index: i + 1))
                    .All(t => t.x == generatedSequence[t.index] * sizes.Item[t.index].Item);
            })
            .When(2 <= sizes.Item.Length);
    }

    [Property]
    public Property ItemIsEqualToThePreviousGeneratedValueAndOriginalSizeSizeIs3()
    {
        var arb = Arb.Default.PositiveInt()
            .Generator
            .Select(x => x.Item)
            .ArrayOf(3)
            .ToArbitrary();

        return Prop.ForAll(arb, sizes =>
        {
            var generatedSequence = CalculateSizes(sizes);

            return new Func<bool>(() =>
            {
                return generatedSequence
                    .SkipLast(1)
                    .Select((x, i) => (x, index: i + 1))
                    .All(t => t.x == generatedSequence[t.index] * sizes[t.index]);
            }).ToProperty();
        });
    }

    private BigInteger[] CalculateSizes(int[] sizes)
    {
        if (sizes.Length <= 1)
        {
            return [1];
        }

        var result = new BigInteger[sizes.Length];

        result[^1] = 1;

        for (var i = 2; i <= result.Length; i++)
        {
            result[^i] = result[^(i - 1)] * sizes[^(i - 1)];
        }

        return result;
    }

    private BigInteger CalculateMaximumNumber(IEnumerable<int> sizes)
    {
        return sizes
            .Aggregate(BigInteger.One, (r, x) => r * x);
    }

    private static int[] GetSizes(int numbersLength)
    {
        return Enumerable
            .Range(0, numbersLength)
            .Select(_ => 2)
            .ToArray();
    }
}