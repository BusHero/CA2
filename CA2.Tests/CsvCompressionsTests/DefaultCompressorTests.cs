namespace CA2.Tests.CsvCompressionsTests;

using GeneratorLibrary.Compression;

public sealed class DefaultCompressorTests
{
    [Theory, AutoData]
    public async Task CsvOptimizer(string[][] csv)
    {
        using var stream = new MemoryStream();

        var compressor = new DefaultCompressor();
        
        await compressor.CompressAsync(csv, stream);
    }
}