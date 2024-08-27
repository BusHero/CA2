using System.Numerics;
using FluentAssertions;
using FsCheck;
using FsCheck.Xunit;

namespace CA2.Tests;

public sealed class GeneratorTests
{
    public static TheoryData<Combination, int> Data => new()
    {
        { new Combination { Item = [0, 0], Sizes = [2, 2] }, 0 },
        { new Combination { Item = [0, 1], Sizes = [2, 2] }, 1 },
        { new Combination { Item = [1, 0], Sizes = [2, 2] }, 2 },
        { new Combination { Item = [1, 1], Sizes = [2, 2] }, 3 },

        { new Combination { Item = [0, 0], Sizes = [3, 2] }, 0 },
        { new Combination { Item = [0, 1], Sizes = [3, 2] }, 1 },
        { new Combination { Item = [1, 0], Sizes = [3, 2] }, 2 },
        { new Combination { Item = [1, 1], Sizes = [3, 2] }, 3 },
        { new Combination { Item = [2, 0], Sizes = [3, 2] }, 4 },
        { new Combination { Item = [2, 1], Sizes = [3, 2] }, 5 },

        { new Combination { Item = [0, 0], Sizes = [2, 3] }, 0 },
        { new Combination { Item = [0, 1], Sizes = [2, 3] }, 1 },
        { new Combination { Item = [0, 2], Sizes = [2, 3] }, 2 },
        { new Combination { Item = [1, 0], Sizes = [2, 3] }, 3 },
        { new Combination { Item = [1, 1], Sizes = [2, 3] }, 4 },
        { new Combination { Item = [1, 2], Sizes = [2, 3] }, 5 },

        { new Combination { Item = [0, 0], Sizes = [3, 3] }, 0 },
        { new Combination { Item = [0, 1], Sizes = [3, 3] }, 1 },
        { new Combination { Item = [0, 2], Sizes = [3, 3] }, 2 },
        { new Combination { Item = [1, 0], Sizes = [3, 3] }, 3 },
        { new Combination { Item = [1, 1], Sizes = [3, 3] }, 4 },
        { new Combination { Item = [1, 2], Sizes = [3, 3] }, 5 },
        { new Combination { Item = [2, 0], Sizes = [3, 3] }, 6 },
        { new Combination { Item = [2, 1], Sizes = [3, 3] }, 7 },
        { new Combination { Item = [2, 2], Sizes = [3, 3] }, 8 },

        { new Combination { Item = [0, 0, 0], Sizes = [2, 2, 2] }, 0 },
        { new Combination { Item = [0, 0, 1], Sizes = [2, 2, 2] }, 1 },
        { new Combination { Item = [0, 1, 0], Sizes = [2, 2, 2] }, 2 },
        { new Combination { Item = [0, 1, 1], Sizes = [2, 2, 2] }, 3 },
        { new Combination { Item = [1, 0, 0], Sizes = [2, 2, 2] }, 4 },
        { new Combination { Item = [1, 0, 1], Sizes = [2, 2, 2] }, 5 },
        { new Combination { Item = [1, 1, 0], Sizes = [2, 2, 2] }, 6 },
        { new Combination { Item = [1, 1, 1], Sizes = [2, 2, 2] }, 7 },
        { new Combination { Item = [1, 1, 0, 0], Sizes = [2, 2, 2, 2] }, 12 },
        { new Combination { Item = [1, 2, 1, 1, 1, 0, 0], Sizes = [2, 7, 3, 2, 2, 2, 2] }, 460 },
        { new Combination { Item = [1, 0, 0, 1, 0, 0, 1, 4, 1], Sizes = [2, 2, 2, 2, 2, 2, 2, 7, 2] }, 1_031 },
        { new Combination { Item = [1, 2, 1], Sizes = [2, 3, 2] }, 11 }
    };

    [Theory]
    [MemberData(nameof(Data))]
    public void GenerateGeneratesExpectedNumber(
        Combination combination,
        int expectedResult)
    {
        var number = Generator.Generate(
            combination.Item,
            combination.Sizes);

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

                var property = 0 <= number;
                
                return property
                    .Label($"{number} should be bigger than 0");
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

                var powerOfTwo = BigInteger.Pow(2, numbers.Length);
                
                var property = number < powerOfTwo;

                return property
                    .Label($"{number} < {powerOfTwo}");
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

            var property = number == 0;
            
            return property
                .Label($"{number} == 0");
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

            var biggestPossibleNumber = BigInteger.Pow(2, numbers.Length) - 1;
            var property = number == biggestPossibleNumber;
            return property.Label($"{number} == {biggestPossibleNumber}");
        });
    }

    [Property(Arbitrary = [typeof(Generators)])]
    public Property NumberIsSmallerThanMaximumPossibleSize(Combination combination)
    {
        var result = Generator.Generate(
            combination.Item,
            combination.Sizes);

        var maximumPossibleNumber = CalculateMaximumNumber(combination.Sizes);

        var property = result <= maximumPossibleNumber;
        
        return property
            .Label($"{result} is smaller than {maximumPossibleNumber}");
    }

    [Theory, MemberData(nameof(Sizes))]
    public void MaximumNumberIsCalculatedCorrectly(int[] sizes, BigInteger expectedResult)
    {
        var result = CalculateMaximumNumber(sizes);

        result.Should().Be(expectedResult);
    }

    public static TheoryData<int[], BigInteger> Sizes => new()
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
        var actualSizes = sizes.Item
            .Select(x => x.Item)
            .ToArray();
        
        var generatedSequence = CalculateSizes(actualSizes);

        var property = () => generatedSequence[^1] == 1;

        return property
            .Label("Last item is 1");
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

            var property = generatedSequence[0] == sizes[^1];
            
            return property
                .Label("Last item is equal to the last one");
        });
    }

    [Property]
    public Property GeneratedArrayIsArrangedDescending(NonEmptyArray<PositiveInt> sizes)
    {
        var actualSizes = sizes.Item
            .Select(x => x.Item)
            .ToArray();
        
        var generatedSequence = CalculateSizes(actualSizes);

        return generatedSequence
            .OrderDescending()
            .SequenceEqual(generatedSequence)
            .Label($"The generated array should be sorted [{string.Join(", ", generatedSequence)}]");
    }

    [Property]
    public Property SizeOfGeneratedArrayIsEqualToTheOriginalSize(NonEmptyArray<PositiveInt> sizes)
    {
        var actualSizes = sizes.Item
            .Select(x => x.Item)
            .ToArray();
        
        var generatedSequence = CalculateSizes(actualSizes);

        var property = generatedSequence.Length == sizes.Item.Length;
        
        return property
            .Label($"{generatedSequence.Length} == {sizes.Item.Length}");
    }

    [Property]
    public Property FirstItemIsTheProductOfPreviousNumbers(NonEmptyArray<PositiveInt> sizes)
    {
        var actualSizes = sizes.Item
            .Select(x => x.Item)
            .ToArray();

        var generatedSequence = CalculateSizes(actualSizes);

        var property = () => generatedSequence[0] == sizes.Item.Skip(1)
            .Select(x => x.Item)
            .Select(x => (BigInteger)x)
            .Aggregate(BigInteger.One, (fst, snd) => fst * snd);
        
        return property
            .When(2 <= sizes.Item.Length);
    }

    [Property(MaxTest = 10_000)]
    public Property ItemIsEqualToThePreviousGeneratedValueAndOriginalSize(NonEmptyArray<PositiveInt> sizes)
    {
        var actualSizes = sizes.Item
            .Select(x => x.Item)
            .ToArray();

        var generatedSequence = CalculateSizes(actualSizes);

        var property = () => generatedSequence
            .SkipLast(1)
            .Select((x, i) => (x, index: i + 1))
            .All(t => t.x == generatedSequence[t.index] * sizes.Item[t.index].Item);
        
        return property
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

            var property = () => generatedSequence
                .SkipLast(1)
                .Select((x, i) => (x, index: i + 1))
                .All(t => t.x == generatedSequence[t.index] * sizes[t.index]);
            
            return property
                .ToProperty();
        });
    }

    private static BigInteger[] CalculateSizes(int[] sizes)
    {
        switch (sizes.Length)
        {
            case 0:
                return [];
            case 1:
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

    private static BigInteger CalculateMaximumNumber(IEnumerable<int> sizes) =>
        sizes
            .Aggregate(BigInteger.One, (r, x) => r * x);

    private static int[] GetSizes(int numbersLength) =>
        Enumerable
            .Range(0, numbersLength)
            .Select(_ => 2)
            .ToArray();
}