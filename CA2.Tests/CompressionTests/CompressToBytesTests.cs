namespace CA2.Tests.CompressionTests;

using System.Numerics;

using GeneratorLibrary.Compression;

using Utils;

public sealed class CompressToBytesTests
{
    private readonly Compressor _compressor = new();

    [Property(Arbitrary = [typeof(CombinationsGenerator)])]
    public Property ArrayProducedIsEnoughToStoreTheBiggestNumber(Combination combination)
    {
        var bytes = _compressor
            .CompressToBytes(combination.Item, combination.Sizes);

        var biggestNumber = TestUtils.CalculateMaximumNumber(combination.Sizes);
        var bytesCount = biggestNumber.GetByteCount();

        return (bytesCount == bytes.Length).ToProperty();
    }

    [Property(Arbitrary = [typeof(CombinationsGenerator)])]
    public Property CanConvertBytesBackToBigNumber(Combination combination)
    {
        var number = _compressor.Compress(
            combination.Item,
            combination.Sizes);

        var bytes = _compressor
            .CompressToBytes(combination.Item, combination.Sizes);
        var newNumber = new BigInteger(bytes);

        return (newNumber == number)
            .Label($"{number} == {newNumber}(0x{string.Join("", bytes.Select(x => x.ToString("x")))})");
    }

    [Property(Arbitrary = [typeof(CombinationsGenerator)])]
    public Property CompressStream_ArrayProducedIsEnoughToStoreTheBiggestNumber(Combination combination)
    {
        using var stream = new MemoryStream();

        _compressor
            .Compress(combination.Item, combination.Sizes, stream);
        var bytes = stream.ToArray();

        var biggestNumber = TestUtils.CalculateMaximumNumber(combination.Sizes);
        var bytesCount = biggestNumber.GetByteCount();

        return (bytesCount == bytes.Length).ToProperty();
    }

    [Property(Arbitrary = [typeof(CombinationsGenerator)])]
    public Property CompressStream_CanConvertBytesBackToBigNumber(Combination combination)
    {
        using var stream = new MemoryStream();

        var number = _compressor.Compress(
            combination.Item,
            combination.Sizes);

        _compressor
            .Compress(combination.Item, combination.Sizes, stream);
        var bytes = stream.ToArray();

        var newNumber = new BigInteger(bytes);

        return (newNumber == number)
            .Label($"{number} == {newNumber}(0x{string.Join("", bytes.Select(x => x.ToString("x")))})");
    }

    [Property(Arbitrary = [typeof(CombinationsGenerator)])]
    public Property CompressStream_CanDecompress(Combination combination)
    {
        using var stream = new MemoryStream();

        _compressor.Compress(combination.Item, combination.Sizes, stream);

        stream.Position = 0;

        var item = _compressor.Decompress(combination.Sizes, stream);

        return (item.Length == 1)
            .And(item[0].SequenceEqual(combination.Item));
    }
}
