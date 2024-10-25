using System.IO.Abstractions;

using Cocona;

namespace CsvGenerator.Console;

public sealed class CsvGeneratorCommand(ICsvGeneratorFactory csvGenerator, IFileSystem fileSystem)
{
    public async Task Command(
        [Option('r')] int rows,
        [Option("column", ['c'])] string[] columns,
        [Option('o')] string? filename,
        [Option('d')] string? destination)
    {
        var realColumns = columns
            .Select(x => x.Split(',').ToArray())
            .ToArray();

        var generator = csvGenerator
            .Create()
            .WithRowsCount(rows)
            .WithColumns(realColumns);

        await ((filename, destination) switch
        {
            (null, null) => WriteToConsole(generator),
            _ => WriteToFile(generator, fileSystem, filename!, destination),
        });
    }

    private static async Task WriteToFile(ICsvGenerator csvGenerator,
        IFileSystem fileSystem,
        string filename,
        string? destination)
    {
        destination ??= fileSystem.Directory.GetCurrentDirectory();

        fileSystem.Directory.CreateDirectory(destination);

        await using var stream = fileSystem.File.CreateText(
            Path.Combine(destination, $"{filename}.csv"));

        await csvGenerator.GenerateAsync(stream);
    }

    private static async Task WriteToConsole(ICsvGenerator csvGenerator)
        => await csvGenerator.GenerateAsync(System.Console.Out);
}