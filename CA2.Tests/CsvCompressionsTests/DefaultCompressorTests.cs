namespace CA2.Tests.CsvCompressionsTests;

using System.Text.Json;

using CA2.Tests.Utils;

using GeneratorLibrary.Compression;

public sealed class DefaultCompressorTests
{
    private readonly Compressor compressor = new();

    [Theory, AutoData]
    public async Task CsvOptimizer(string[][] csv)
    {
        using var stream = new MemoryStream();

        var compressor = new DefaultCompressor();

        await compressor.CompressAsync(csv, stream);
    }

    [Property(Arbitrary = [typeof(CombinationsGenerator)])]
    public void StreamContainsExpectedNumberOfBytes(RealCombination combination)
    {
        using var stream = new MemoryStream();

        compressor.Compress(combination.Items, combination.Sizes, stream);

        stream.Should().HaveLength(combination.Sizes.CalculateMaximumNumber().GetByteCount() * combination.Items.Length);
    }

    [Property(Arbitrary = [typeof(CombinationsGenerator)])]
    public void DecompressFromStream(RealCombination combination)
    {
        using var stream = new MemoryStream();

        compressor.Compress(combination.Items, combination.Sizes, stream);
        stream.Position = 0;

        var rows = compressor.Decompress(combination.Sizes, stream);

        JsonSerializer.Serialize(rows).Should().BeEquivalentTo(JsonSerializer.Serialize(combination.Items));
    }
}
