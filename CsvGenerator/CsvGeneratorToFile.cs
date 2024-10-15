using System.IO.Abstractions;

namespace CsvGenerator;

public class CsvGeneratorToFile(
    IFileSystem fileSystem,
    IRandomCsvGeneratorFactory factory)
{
    public async Task GenerateAsync(Stream writer, int rowsCount, string[][] columns)
        => await GenerateAsync(new StreamWriter(writer), rowsCount, columns);

    public async Task GenerateAsync(
        string destinationFolder,
        string filename,
        int rowsCount,
        string[][] columns)
    {
        fileSystem.Directory.CreateDirectory(destinationFolder);

        using var stream = fileSystem.File.CreateText(Path.Combine(destinationFolder, $"{filename}.csv"));

        await GenerateAsync(stream, rowsCount, columns);
    }

    public async Task GenerateAsync(StreamWriter writer, int rowsCount, string[][] columns)
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