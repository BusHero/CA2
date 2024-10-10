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
        PositiveInt samples,
        NonEmptyArray<PositiveInt> sizes)
    {
        var realSizes = sizes
            .Get
            .Select(x => x.Get)
            .Select(x => 2 <= x ? x : 2)
            .ToArray();

        var actualRows = Arb.Default.PositiveInt()
            .Generator
            .Select(x => x.Get)
            .ArrayOf(sizes.Get.Length)
            .Select(x => x.Zip(realSizes, (nbr, size) => nbr % size).ToArray())
            .Sample(100, samples.Get)
            .ToArray();

        using var stream = new MemoryStream();

        compressor.Compress(actualRows, realSizes, stream);

        stream.Should().HaveLength(realSizes.CalculateMaximumNumber().GetByteCount() * samples.Get);
    }
}