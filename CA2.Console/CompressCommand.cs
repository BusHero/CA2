using System.IO.Abstractions;

using Cocona;

using CA2.Compression;
using CA2.Extractors;

namespace CA2.Console;

public sealed class CompressCommand(
    IFileSystem fileSystem,
    IReadOnlyCollection<IExtractor> extractors,
    ICompressor csvCompressor)
{
    public async Task Command(string format,
        [Option('i')] string? input,
        [Option('o')] string? output,
        [Option("column", ['c'])] int[] columns,
        [Option('t')] byte strength)
    {
        var csv =  await GetCsv(format, input);

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

    private async Task<int[][]> GetCsv(
        string format, 
        string? inputFile)
    {
        var extractor = extractors.First(x => string.Equals(x.Format, format, StringComparison.OrdinalIgnoreCase));
        
        if (inputFile == null)
        {
            return await extractor.ExtractAsync(System.Console.In);
        }

        using var inputStream = fileSystem.File.OpenText(inputFile);

        return await extractor.ExtractAsync(inputStream);
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