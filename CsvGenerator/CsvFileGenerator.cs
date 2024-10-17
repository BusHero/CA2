namespace CsvGenerator;

public class CsvFileGenerator(
    IRandomCsvGeneratorFactory factory) : ICsvFileGenerator
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