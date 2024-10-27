namespace CA2.Tests.Extractors;

using System.Threading.Tasks;

using CA2.Extractors;

public sealed class ActsExtractorTests
{
    [Fact]
    public async Task ResultShouldNotBeNullOrEmpty()
    {
        var csvResult = await OptimizeCsvAsync("""
            0,0,1
            0,1,0
            1,0,0
            1,1,1
            """);
        var actsResult = await OptimizeActsAsync("""
            4
            0 0 1 
            0 1 0 
            1 0 0 
            1 1 1 

            """);

        actsResult.Should().BeEquivalentTo(csvResult);
    }

    private async Task<int[][]> OptimizeActsAsync(string acts)
    {
        using var stream = new MemoryStream();
        using var writer = new StreamWriter(stream);

        await writer.WriteAsync(acts);
        await writer.FlushAsync();
        stream.Position = 0;
        using var reader = new StreamReader(stream);
        var extractor = new ActsExtractor();

        var result = await extractor.ExtractAsync(reader);

        return result;
    }

    private async Task<int[][]> OptimizeCsvAsync(string csv)
    {
        using var stream = new MemoryStream();
        using var writer = new StreamWriter(stream);

        await writer.WriteAsync(csv);
        await writer.FlushAsync();
        stream.Position = 0;
        using var reader = new StreamReader(stream);
        var extractor = new CsvExtractor();

        var result = await extractor.ExtractAsync(reader);

        return result;
    }
}
