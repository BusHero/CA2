namespace CA2.Tests.Extractors;
using CsvGenerator;

public class CsvExtractorTests
{
    private readonly ICsvGenerator _generator = new CsvGenerator(new DefaultRandomCsvGeneratorFactory());

    [Fact]
    public async Task Foo()
    {
        using var stream = new MemoryStream();
        await _generator.GenerateAsync(stream, 100, [[]]);
        var extractor = new CsvExtractor();

        await extractor.ExtractAsync(new MemoryStream());
    }
}