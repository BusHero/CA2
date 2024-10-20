using System.IO.Abstractions;

using Cocona;

namespace CsvGenerator.Console;

public class CsvGeneratorCommand(ICsvGenerator csvGenerator, IFileSystem fileSystem)
{
    public async Task Command(
        [Option('r')]int rows,
        [Option("column", ['c'])] string[] columns,
        [Option('o')]string? filename,
        [Option('d')]string? destination)
    {
        var realColumns = columns
            .Select(x => x.Split(',').ToArray())
            .ToArray();

        await ((filename, destination) switch
        {
            (null, null) => WriteToConsole(
                csvGenerator, 
                rows, 
                realColumns),
            _ => WriteToFile(
                csvGenerator, 
                fileSystem, 
                rows, 
                filename!, 
                destination, 
                realColumns),
        });
    }

    private static async Task WriteToFile(ICsvGenerator csvGenerator,
        IFileSystem fileSystem,
        int rows,
        string filename,
        string? destination,
        string[][] realColumns)
    {
        await using var stream = GetStream(
            fileSystem,
            filename,
            destination);

        await csvGenerator.GenerateAsync(
            stream,
            rows,
            realColumns);
    }

    private static async Task WriteToConsole(ICsvGenerator csvGenerator,
        int rows,
        string[][] realColumns)
        => await csvGenerator.GenerateAsync(
            System.Console.Out,
            rows,
            realColumns);

    private static TextWriter GetStream(
        IFileSystem fileSystem,
        string filename,
        string? destination)
    {
        destination ??= fileSystem.Directory.GetCurrentDirectory();

        fileSystem.Directory.CreateDirectory(destination);

        var stream = fileSystem.File.CreateText(
            Path.Combine(destination, $"{filename}.csv"));

        return stream;
    }
}