using System.IO.Abstractions;

using GeneratorLibrary.Compression;

namespace GeneratorLibrary;

public class CcaGenerator(
    IFileSystem fileSystem,
    ICompressor csvCompressor)
{
    public async Task GenerateCcaFile(string csvFile, int[] sizes)
    {
        var ccaFilename = GetCcaFilename(csvFile);

        var csv = GetCsv(csvFile);
        await using var file = fileSystem.File.Create(ccaFilename);
        
        await csvCompressor.CompressAsync(csv, sizes, file);
    }

    private string[][] GetCsv(string inputFile)
    {
        var content = fileSystem.File.ReadAllLines(inputFile)
            .Select(x => x.Split(','))
            .ToArray();
        return content;
    }

    private static string GetCcaFilename(string inputFile)
    {
        var result = Path.GetFileNameWithoutExtension(inputFile);
        var outputFile = $"{result}.cca";

        return outputFile;
    }
}