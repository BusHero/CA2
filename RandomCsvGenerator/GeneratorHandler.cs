namespace RandomCsvGenerator;

using GeneratorLibrary.CsvGenerators;

public static class GeneratorHandler
{
    public static async Task HandleAsync(
        string destination,
        string filename,
        int rowsCount,
        string[][] columns)
    {
        var csv = new RandomCsvGenerator()
            .WithColumns(columns)
            .WithRowsCount(rowsCount)
            .Generate();

        var path = Path.Combine(destination, $"{filename}.csv");
        await WriteCsvFile(path, csv);
    }

    private static async Task WriteCsvFile(
        string outputFile,
        string[][] strings)
    {
        // Use a logger instead.
        Console.WriteLine($"Write to {outputFile}");

        var rows = strings
            .Select(x => string.Join(',', x))
            .ToList();

        foreach (var row in rows)
        {
            Console.WriteLine(row);
        }

        await File.WriteAllLinesAsync(outputFile, rows);
    }
}