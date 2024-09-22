namespace RandomCsvGenerator;

internal static class GeneratorHandler
{
    public static async Task HandleAsync(
        string destination,
        string filename,
        int rowsCount,
        string[][] columns)
    {
        var csv = new GeneratorLibrary.RandomCsvGenerator()
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