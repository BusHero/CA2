using System.Numerics;

using GeneratorLibrary;

namespace CA2.Tests.GeneratorTests;

public sealed class GenerateTests
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
        { new Combination { Item = [1, 2, 1], Sizes = [2, 3, 2] }, 11 },
    };

    [Theory, AutoData,]
    public void ValuesIsNullThrows(int[] sizes)
    {
        var func = () => Generator.Generate(
            null!,
            sizes);

        func.Should()
            .Throw<ArgumentNullException>();
    }

    [Theory, AutoData]
    public void SizesIsNullThrows(int[] values)
    {
        var func = () => Generator.Generate(
            values,
            null!);

        func.Should()
            .Throw<ArgumentNullException>();
    }

    [Fact]
    public void ValuesLengthIsDifferentFromSizesLength_ThrowInvalidOperationException()
    {
        int[] values = [1, 1, 1];
        int[] sizes = [4, 4];

        var func = () => Generator.Generate(
            values,
            sizes);

        func.Should()
            .Throw<InvalidOperationException>();
    }

    [Fact]
    public void ValuesContainNegativeNumbers_ThrowInvalidOperationException()
    {
        int[] values = [-1, 1, 1];
        int[] sizes = [4, 4, 4];

        var func = () => Generator.Generate(
            values,
            sizes);

        func.Should()
            .Throw<InvalidOperationException>();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    public void SizesContainElementsSmallerOrEqualTo1_ThrowInvalidOperationException(int size)
    {
        var prop = () => Generator.Generate(
            [0],
            [size]);

        prop.Should()
            .Throw<InvalidOperationException>();
    }

    [Theory]
    [InlineData(
        10,
        10)]
    [InlineData(
        11,
        10)]
    public void ValuesContainElementsBiggerThanSizes_ThrowInvalidOperationException(int value, int size)
    {
        var property = () => Generator.Generate(
            [value],
            [size]);

        property.Should()
            .Throw<InvalidOperationException>();
    }

    [Theory]
    [MemberData(nameof(Data))]
    public void GenerateGeneratesExpectedNumber(
        Combination combination,
        int expectedResult)
    {
        var number = Generator.Generate(
            combination.Item,
            combination.Sizes);

        number.Should()
            .Be(expectedResult);
    }

    [Property]
    public Property ResultIsBiggerOrEqualToZero()
    {
        var arb = Gen
            .Sized(
                size => Gen
                    .Elements(
                        0,
                        1)
                    .ArrayOf(size))
            .ToArbitrary();

        return Prop
            .ForAll(
                arb,
                numbers =>
                {
                    var sizes = GetSizes(numbers.Length);

                    var number = Generator.Generate(
                        numbers,
                        sizes);

                    var property = 0 <= number;

                    return property
                        .Label($"{number} should be bigger than 0");
                });
    }

    [Property]
    public Property ResultIsSmallerOrEqualToTheCount()
    {
        var arb = Gen
            .Sized(
                size => Gen
                    .Elements(
                        0,
                        1)
                    .ArrayOf(size))
            .ToArbitrary();

        return Prop
            .ForAll(
                arb,
                numbers =>
                {
                    var sizes = GetSizes(numbers.Length);

                    var number = Generator.Generate(
                        numbers,
                        sizes);

                    var powerOfTwo = BigInteger.Pow(
                        2,
                        numbers.Length);

                    var property = number < powerOfTwo;

                    return property
                        .Label($"{number} < {powerOfTwo}");
                });
    }

    [Property]
    public Property ArraysOfZerosHaveZero()
    {
        var arb = Gen
            .Sized(
                size => Gen
                    .Elements(0)
                    .ArrayOf(size))
            .ToArbitrary();

        return Prop.ForAll(
            arb,
            numbers =>
            {
                var sizes = GetSizes(numbers.Length);

                var number = Generator.Generate(
                    numbers,
                    sizes);

                var property = number == 0;

                return property
                    .Label($"{number} == 0");
            });
    }

    [Property]
    public Property ArraysOfOnesHaveMaxPower()
    {
        var arb = Gen
            .Sized(
                size => Gen
                    .Elements(1)
                    .ArrayOf(size))
            .ToArbitrary();

        return Prop.ForAll(
            arb,
            numbers =>
            {
                var sizes = GetSizes(numbers.Length);

                var number = Generator.Generate(
                    numbers,
                    sizes);

                var biggestPossibleNumber = BigInteger.Pow(
                                                2,
                                                numbers.Length)
                                            - 1;
                var property = number == biggestPossibleNumber;

                return property.Label($"{number} == {biggestPossibleNumber}");
            });
    }

    [Property(Arbitrary = [typeof(CombinationsGenerator)])]
    public Property NumberIsSmallerThanMaximumPossibleSize(Combination combination)
    {
        var result = Generator.Generate(
            combination.Item,
            combination.Sizes);

        var maximumPossibleNumber = TestUtils.CalculateMaximumNumber(combination.Sizes);

        var property = result <= maximumPossibleNumber;

        return property
            .Label($"{result} is smaller than {maximumPossibleNumber}");
    }

    private static int[] GetSizes(int numbersLength) =>
        Enumerable
            .Range(
                0,
                numbersLength)
            .Select(_ => 2)
            .ToArray();
}
