namespace CsvGenerator.Console.Tests;

using System.IO.Abstractions;
using System.Threading.Tasks;

using Cocona;

public static class CsvGeneratorCommand
{
    public static async Task Command(
        [FromService] ICsvFileGenerator csvFileGenerator,
        [FromService] IFileSystem fileSystem,
        int rows,
        string[] columns,
        string filename,
        string? destination = null)
    {
        System.Console.WriteLine("Here we go");

        var realColumns = columns
            .Select(x => x.Split(',').ToArray())
            .ToArray();

        await using var stream = CreateFile(fileSystem, filename, destination);

        await csvFileGenerator.GenerateAsync(stream, rows, realColumns);
    }

    private static StreamWriter CreateFile(IFileSystem fileSystem, string filename, string? destination)
    {
        destination ??= fileSystem.Directory.GetCurrentDirectory();

        fileSystem.Directory.CreateDirectory(destination);

        var stream = fileSystem.File.CreateText(
            Path.Combine(destination, $"{filename}.csv"));

        return stream;
    }
}