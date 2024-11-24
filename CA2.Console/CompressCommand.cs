using System.IO.Abstractions;

using CA2.Compression;
using CA2.Extractors;

using Cocona;

namespace CA2.Console;

public sealed class CompressCommand(
    IFileSystem fileSystem,
    IEnumerable<IExtractor> extractors,
    ICompressor csvCompressor)
{
    private readonly IReadOnlyCollection<IExtractor> _extractors = extractors.ToList();

    public async Task Command(
        string format,
        [Option('i')] string? input,
        [Option('o')] string? output,
        [Option("column", ['c'])] int[] columns,
        [Option('t')] byte strength,
        CancellationToken token = default)
    {
        var csv = await GetCsv(format, input);

        output ??= input ?? "test";

        var parent = fileSystem.Path.GetDirectoryName(input)!;

        var ccaFullFilename = fileSystem.Path.Combine(parent, fileSystem.Path.ChangeExtension(output, "cca"));
        var ccaMetaFilename = fileSystem.Path.Combine(parent, fileSystem.Path.ChangeExtension(output, "ccmeta"));

        await using var ccaFile = fileSystem.File.Create(ccaFullFilename);
        await using var metaFile = fileSystem.File.Create(ccaMetaFilename);

        await csvCompressor.WriteCcaAsync(
            csv,
            columns,
            ccaFile,
            token);
        csvCompressor.WriteMetadata(
            csv.Length,
            columns,
            strength,
            metaFile);
    }

    private async Task<int[][]> GetCsv(
        string format,
        string? inputFile)
    {
        var extractor = _extractors.First(x => string.Equals(x.Format, format, StringComparison.OrdinalIgnoreCase));

        if (inputFile == null)
        {
            return await extractor.ExtractAsync(System.Console.In);
        }

        using var inputStream = fileSystem.File.OpenText(inputFile);

        return await extractor.ExtractAsync(inputStream);
    }
}