namespace CA2.Tests.CsvGenerationTests;

using System.IO.Abstractions.TestingHelpers;

using GeneratorLibrary;

public class CsvGeneratorToFileTests
{
    [Theory, AutoData]
    public async Task FileIsCreated(
        string filename,
        int rowsCount,
        string[][] columns)
    {
        var fileSystem = new MockFileSystem();

        var generator = new CsvGeneratorToFile(fileSystem);

        await generator.Generate(
            ".",
            filename,
            rowsCount,
            columns);

        fileSystem
            .File
            .Exists(filename)
            .Should()
            .BeTrue();
    }

    [Theory, AutoData]
    public async Task FileInSpecifiedFolderIsCreated(
        string folder,
        string filename,
        int rowsCount,
        string[][] columns)
    {
        var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
        {
            [folder] = new MockDirectoryData(),
        });

        var generator = new CsvGeneratorToFile(fileSystem);

        await generator.Generate(
            folder,
            filename,
            rowsCount,
            columns);

        fileSystem
            .File
            .Exists(Path.Combine(folder, filename))
            .Should()
            .BeTrue();
    }

    [Theory, AutoData]
    public async Task Generate_NonExistingFolder_IsCreated(
        string folder,
        string filename,
        int rowsCount,
        string[][] columns)
    {
        var fileSystem = new MockFileSystem();

        var generator = new CsvGeneratorToFile(fileSystem);

        await generator.Generate(
            folder,
            filename,
            rowsCount,
            columns);

        fileSystem
            .File
            .Exists(Path.Combine(folder, filename))
            .Should()
            .BeTrue();
    }

    [Theory, AutoData]
    public async Task Generate_ExistingDestinationFilename_DontThrow(
        string folder,
        string filename,
        int rowsCount,
        string[][] columns)
    {
        var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
        {
            [Path.Combine(folder, filename)] = new(string.Empty),
        });

        var generator = new CsvGeneratorToFile(fileSystem);

        await generator.Generate(
            folder,
            filename,
            rowsCount,
            columns);

        fileSystem
            .File
            .Exists(Path.Combine(folder, filename))
            .Should()
            .BeTrue();
    }
}