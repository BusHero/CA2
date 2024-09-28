namespace CA2.Tests.CcaGenerationTests;

using System.IO.Abstractions.TestingHelpers;
using GeneratorLibrary;
using Utils;

public sealed class CcaGeneratorTests
{
    [Property]
    public Property CcaFileGetsGenerated(
        Guid filename,
        NonEmptyArray<PositiveInt> columns)
    {
        const int rowsCount = 10000;
        var inputFile = filename.ToString();
        var realColumns = columns.Get
            .Select(x => x.Get + 2)
            .ToArray();
        var bytesCount = TestUtils
            .CalculateMaximumNumber(realColumns)
            .GetByteCount() * rowsCount;

        var csv = new RandomCsvGenerator()
            .WithColumns(realColumns)
            .WithRowsCount(rowsCount)
            .Generate();

        var csvAsContent = csv.ConvertToString();

        var fileSystem = new MockFileSystem();
        fileSystem.File.WriteAllText(
            inputFile,
            csvAsContent);

        var instance = new CcaGenerator(fileSystem);
        instance.GenerateCcaFile(inputFile);

        var stream = fileSystem.File.OpenRead($"{inputFile}.cca");

        return (stream.Length == bytesCount).ToProperty();
    }
}
