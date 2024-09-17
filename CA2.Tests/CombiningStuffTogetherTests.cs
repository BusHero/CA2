namespace CA2.Tests;

using System.IO.Abstractions.TestingHelpers;

using GeneratorLibrary;

public sealed class CombiningStuffTogetherTests
{
    [Theory, AutoData]
    public void OutputFileGetsWritten(string inputFile)
    {
        var fileSystem = new MockFileSystem();
        fileSystem.File.WriteAllText(inputFile, string.Empty);

        var instance = new ClassThatDoesStuff(fileSystem);
        instance.DoStuff(inputFile);

        fileSystem
            .File
            .Exists($"{inputFile}.cca")
            .Should()
            .BeTrue();
    }

    [Property]
    public Property CsvOfASingleLineContainsASingleByte(
        string inputFile,
        PositiveInt rows,
        NonEmptyArray<PositiveInt> columns)
    {
        var realColumns = columns.Get
            .Select(x => x.Get)
            .ToArray();
        
        var csv = new RandomCsvGenerator()
            .WithColumns(realColumns)
            .WithRowsCount(rows.Get)
            .Generate();
        var csvAsContent = string.Join("\n\r", csv.Select(x => string.Join(',', x)));

        var fileSystem = new MockFileSystem();
        fileSystem.File.WriteAllText(inputFile, csvAsContent);

        var instance = new ClassThatDoesStuff(fileSystem);
        instance.DoStuff(inputFile);

        var stream = fileSystem.File.OpenRead($"{inputFile}.cca");

        return (stream.Length == 1).ToProperty();
    }
}