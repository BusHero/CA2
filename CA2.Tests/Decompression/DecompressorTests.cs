using TestUtils;

namespace CA2.Tests.Decompression;

using CA2.Compression;

using Utils;

public sealed class DecompressorTests
{
    [Property(Arbitrary = [typeof(CombinationsGenerator)])]
    public Property DecompressingCompressedNumberReturnsSameCombination(Combination combination)
    {
        var sut = new Compressor();

        var compressedNumber = Compressor.Compress(
            combination.Item,
            combination.Sizes);

        var result = sut.Decompress(
            compressedNumber,
            combination.Sizes);

        return result
            .SequenceEqual(combination.Item)
            .Label(
                $"round robin - [{result.ConvertToString()}] original - [({combination.Item.ConvertToString()})]");
    }
}