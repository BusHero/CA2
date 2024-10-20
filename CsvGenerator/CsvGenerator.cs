namespace CsvGenerator;

public class CsvGenerator(
    IRandomCsvGeneratorFactory factory) : ICsvGenerator
{
    public async Task GenerateAsync(
        TextWriter writer, 
        int rowsCount, 
        string[][] columns)
    {
        var csv = factory
            .Create()
            .WithRowsCount(rowsCount)
            .WithColumns(columns)
            .Generate()
            .Select(x => string.Join(',', x));

        foreach (var row in csv)
        {
            await writer.WriteLineAsync(row);
        }

        await writer.FlushAsync();
    }
}