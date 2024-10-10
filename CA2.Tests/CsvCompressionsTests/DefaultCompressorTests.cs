namespace CA2.Tests.CsvCompressionsTests;

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

    [Property]
    public void StreamContainsExpectedNumberOfBytes(
        NonEmptyArray<PositiveInt> rows,
        NonEmptyArray<PositiveInt> sizes)
    {
        var realSizes = sizes
            .Get
            .Select(x => x.Get)
            .Select(x => 2 <= x ? x : 2)
            .ToArray();


        var actualRows = rows
            .Get
            .Select(x => x.Get)
            .Select(x => realSizes.Select(size => x % size).ToArray())
            .ToArray();

        using var stream = new MemoryStream();

        compressor.Compress(actualRows, realSizes, stream);

        stream.Should().HaveLength(realSizes.CalculateMaximumNumber().GetByteCount() * rows.Get.Length);
    }
}