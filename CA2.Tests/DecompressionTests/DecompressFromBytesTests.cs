namespace CA2.Tests.DecompressionTests;

using CA2.Tests.Utils;

using GeneratorLibrary.Compression;

public class DecompressFromBytesTests
{
    [Property(Arbitrary = [typeof(CombinationsGenerator)])]
    public Property DecompressBytes(Combination combination)
    {
        var compressor = new Compressor();

        var bytes = compressor.CompressToBytes(
            combination.Item, 
            combination.Sizes);

        var item = compressor.Decompress(bytes, combination.Sizes);

        return item.SequenceEqual(combination.Item)
            .ToProperty();
    }
}