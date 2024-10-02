using CA2.Tests.Utils;

namespace CA2.Tests.DecompressionTests;

using System.Numerics;

using GeneratorLibrary;

public class DecompressorTests
{
    private readonly Compressor _sut = new();

    [Property]
    public Property ResultHasSameLengthAsSizes(NonEmptyArray<PositiveInt> input)
    {
        var sizes = input.Get.Select(x => x.Get).ToArray();

        var result = _sut.Decompress(
            BigInteger.Zero,
            sizes);

        return (result.Length == input.Get.Length).ToProperty();
    }

    [Property]
    public Property DecompressingZerosReturnsAnArrayOfZeros(NonEmptyArray<PositiveInt> input)
    {
        var sizes = input.Get.Select(x => x.Get).ToArray();

        var result = _sut.Decompress(
            BigInteger.Zero,
            sizes);

        return result.All(x => x == 0).ToProperty();
    }

    [Property(Arbitrary = [typeof(CombinationsGenerator)])]
    public Property DecompressingCompressedNumberReturnsSameCombination(Combination combination)
    {
        var compressedNumber = _sut.Compress(
            combination.Item,
            combination.Sizes);

        var result = _sut.Decompress(
            compressedNumber,
            combination.Sizes);

        return result
            .SequenceEqual(combination.Item)
            .Label(
                $"round robin - [{result.ConvertToString()}] original - [({combination.Item.ConvertToString()})]");
    }

    public static TheoryData<BigInteger, int[], int[]> DecompressionTheoryData => new()
    {
        { BigInteger.Zero, [2], [0] },
        { BigInteger.One, [2], [1] },
        { 2, [3], [2] },
        
        { 0, [2, 2], [0, 0] },
        { 1, [2, 2], [0, 1] },
        { 2, [2, 2], [1, 0] },
        { 3, [2, 2], [1, 1] },
        
        { 0, [3, 3], [0, 0] },
        { 1, [3, 3], [0, 1] },
        { 2, [3, 3], [0, 2] },
        { 3, [3, 3], [1, 0] },
        { 4, [3, 3], [1, 1] },
        { 5, [3, 3], [1, 2] },
        { 6, [3, 3], [2, 0] },
        { 7, [3, 3], [2, 1] },
        { 8, [3, 3], [2, 2] },
    };

    [Theory, MemberData(nameof(DecompressionTheoryData))]
    public void DecompressionGivesBackExpectedResult(
        BigInteger compressedValue,
        int[] sizes,
        int[] expectedResult)
    {
        var actualResult = _sut.Decompress(
            compressedValue,
            sizes);

        actualResult
            .Should()
            .BeEquivalentTo(
                expectedResult,
                x => x.WithStrictOrdering());
    }

    [Property(Arbitrary = [typeof(CombinationsGenerator)])]
    public Property SizeHasLengthOne_OutputEqualsInput(
        Combination combination)
    {
        var result = _sut.Decompress(
            combination.Item[0],
            [combination.Sizes[0]]);

        return (result[0] == combination.Item[0])
            .ToProperty();
    }
}