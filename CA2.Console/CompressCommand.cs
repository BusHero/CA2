using System.IO.Abstractions;

using Cocona;

using GeneratorLibrary.Compression;

namespace CA2.Console;

public class CompressCommand(
    IFileSystem fileSystem,
    ICompressor csvCompressor)
{
    public async Task Command(
        [Option('i')] string? input,
        [Option('o')] string? output,
        [Option("column", ['c'])] int[] columns,
        [Option('t')] byte strength)
    {
        var csv = await GetCsv(input);

        output ??= input ?? "test";

        await using var ccaFile = fileSystem.File.Create(GetCcaFilename(output));
        await using var metaFile = fileSystem.File.Create(GetCcaMetaFilename(output));

        await csvCompressor.CompressAsync(
            csv,
            columns,
            strength,
            ccaFile,
            metaFile);
    }

    private async Task<string[][]> GetCsv(string? inputFile)
    {
        if (inputFile == null)
        {
            return await GetCsv(System.Console.In);
        }

        using var inputStream = fileSystem.File.OpenText(inputFile);

        return await GetCsv(inputStream);
    }

    private static async Task<string[][]> GetCsv(TextReader reader)
    {
        var text = await reader.ReadToEndAsync();

        var csv = text.Split(System.Environment.NewLine)
            .Select(line => line.Split(',').ToArray())
            .ToArray();

        return csv;
    }

    private string GetCcaFilename(string inputFile)
    {
        var fileWithoutExtension = fileSystem.Path.GetFileNameWithoutExtension(inputFile);

        return $"{fileWithoutExtension}.cca";
    }

    private string GetCcaMetaFilename(string inputFile)
    {
        var fileWithoutExtension = fileSystem.Path.GetFileNameWithoutExtension(inputFile);

        return $"{fileWithoutExtension}.ccmeta";
    }
}