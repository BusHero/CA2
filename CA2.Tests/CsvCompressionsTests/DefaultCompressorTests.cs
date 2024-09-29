namespace CA2.Tests.CsvCompressionsTests;

using GeneratorLibrary;

public sealed class DefaultCompressorTests
{
    [Theory, AutoData]
    public void CsvOptimizer(
        int[][] csv,
        int[] columns)
    {
        var memoryStream = new MemoryStream();
        var compressor = new Generator();

        compressor.CompressAsync(
            csv,
            columns,
            memoryStream);
    }
}