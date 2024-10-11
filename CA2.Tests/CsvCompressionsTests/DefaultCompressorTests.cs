namespace CA2.Tests.CsvCompressionsTests;

using System.Text.Json;

using CA2.Tests.Utils;

using GeneratorLibrary.Compression;
using GeneratorLibrary.CsvGenerators;

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

    [Property]
    public void CompressCSV(PositiveInt rowsCount, NonEmptyArray<PositiveInt> columns)
    {
        var realColumns = GetRealColumns(columns);
        var generator = new RandomCsvGenerator();

        var csv = generator
            .WithColumns(realColumns)
            .WithRowsCount(rowsCount.Get)
            .Generate();

        using var stream = new MemoryStream();

        compressor.Compress(csv, realColumns, stream);

        stream.Should()
            .HaveLength(rowsCount.Get * realColumns.CalculateMaximumNumber().GetByteCount());
    }

    private int[] GetRealColumns(NonEmptyArray<PositiveInt> values)
    {
        return values.Get.Select(x => x.Get).Select(x => 2 < x ? x : 2).ToArray();
    }
}
