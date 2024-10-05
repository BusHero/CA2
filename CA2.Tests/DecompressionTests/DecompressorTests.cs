using CA2.Tests.Utils;

namespace CA2.Tests.DecompressionTests;

using GeneratorLibrary.Compression;

public class DecompressorTests
{
    [Property(Arbitrary = [typeof(CombinationsGenerator)])]
    public Property DecompressingCompressedNumberReturnsSameCombination(Combination combination)
    {
        var sut = new Compressor();

        var compressedNumber = sut.Compress(
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