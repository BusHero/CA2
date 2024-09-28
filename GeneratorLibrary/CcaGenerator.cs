using System.IO.Abstractions;

namespace GeneratorLibrary;

public class CcaGenerator(
    IFileSystem fileSystem)
{
    public void GenerateCcaFile(string inputFile)
    {
        var csv = GetCsv(inputFile);
        var outputFile = GetOutputFile(inputFile);

        var report = CsvOptimizer.Optimize(csv);


        using var stream = fileSystem.File.Create(outputFile);
        var foo = Generator.GetNumberOfBytesForCombination(report.Sizes);

        var result = report.Csv
            .Select(x => Generator.Generate(x, report.Sizes))
            .ToArray();

        Generator.TryWriteToBuffer(
            stream,
            result,
            foo);
    }

    private string[][] GetCsv(string inputFile)
    {
        var content = fileSystem.File.ReadAllLines(inputFile)
            .Select(x => x.Split(','))
            .ToArray();
        return content;
    }

    private string GetOutputFile(string inputFile)
    {
        var result = Path.GetFileNameWithoutExtension(inputFile);
        var outputFile = $"{result}.cca";

        return outputFile;
    }
}