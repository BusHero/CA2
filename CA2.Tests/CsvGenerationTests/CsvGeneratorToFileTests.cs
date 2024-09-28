namespace CA2.Tests.CsvGenerationTests;

using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;

using GeneratorLibrary;

public class CsvGeneratorToFileTests
{
    private readonly FixtureBuilder _builder = new();

    [Theory, AutoData]
    public async Task FileIsCreated(
        string filename,
        int rowsCount,
        string[][] columns)
    {
        var fixture = _builder.Build();

        await fixture.Sut.Generate(
            ".",
            filename,
            rowsCount,
            columns);

        fixture.AssetFileExists(filename);
    }

    [Theory, AutoData]
    public async Task FileInSpecifiedFolderIsCreated(
        string parent,
        string filename,
        int rowsCount,
        string[][] columns)
    {
        var fixture = _builder
            .WithDirectory(parent)
            .Build();

        await fixture.Sut.Generate(
            parent,
            filename,
            rowsCount,
            columns);

        fixture.AssetFileExists(parent, filename);
    }

    [Theory, AutoData]
    public async Task Generate_NonExistingFolder_IsCreated(
        string parent,
        string filename,
        int rowsCount,
        string[][] columns)
    {
        var fixture = _builder.Build();

        await fixture.Sut.Generate(
            parent,
            filename,
            rowsCount,
            columns);

        fixture.AssetFileExists(parent, filename);
    }

    [Theory, AutoData]
    public async Task Generate_ExistingDestinationFilename_DontThrow(
        string parent,
        string filename,
        int rowsCount,
        string[][] columns)
    {
        var fixture = _builder
            .WithFile(parent, filename)
            .Build();

        await fixture.Sut.Generate(
            parent,
            filename,
            rowsCount,
            columns);

        fixture.AssetFileExists(parent, filename);
    }

    [Theory, AutoData]
    public async Task Generate_CsvWithExpectedNumberOfRowsIsCreated(
        string parent,
        string filename,
        int rowsCount,
        string[][] columns,
        string[][] csv)
    {
        var fixture = _builder
            .WithFile(parent, filename)
            .WithRandomCsv(csv)
            .Build();

        await fixture.Sut.Generate(
            parent,
            filename,
            rowsCount,
            columns);

        await fixture.AssetGeneratedCsvContainsExpectedCsv(
            parent, 
            filename,
            csv);
    }

    private class FixtureBuilder
    {
        private readonly Dictionary<string, MockFileData> _fileData = [];
        private readonly RandomCsvGenerator _csvGenerator = Substitute.For<RandomCsvGenerator>();

        public Fixture Build()
        {
            var fileSystem = new MockFileSystem(_fileData);

            var sut = new CsvGeneratorToFile(
                fileSystem,
                _csvGenerator);

            return new Fixture(
                sut,
                fileSystem);
        }

        public FixtureBuilder WithDirectory(string folder)
        {
            _fileData[folder] = new MockDirectoryData();

            return this;
        }

        public FixtureBuilder WithFile(string parent, string filename)
            => WithFile(Path.Combine(parent, filename));

        private FixtureBuilder WithFile(string filename)
        {
            _fileData[filename] = new MockFileData(string.Empty);

            return this;
        }

        public FixtureBuilder WithRandomCsv(string[][] csv)
        {
            _csvGenerator.Generate().Returns(csv);

            return this;
        }
    }

    private class Fixture(
        CsvGeneratorToFile sut,
        IFileSystem fileSystem)
    {
        public CsvGeneratorToFile Sut { get; } = sut;

        public void AssetFileExists(string parent, string filename)
            => AssetFileExists(Path.Combine(parent, filename));

        public void AssetFileExists(string filename)
        {
            fileSystem
                .File
                .Exists(filename)
                .Should()
                .BeTrue();
        }

        public async Task AssetGeneratedCsvContainsExpectedCsv(
            string parent,
            string filename,
            string[][] csv)
        {
            var actualLines = (await fileSystem
                    .File
                    .ReadAllLinesAsync(Path.Combine(parent, filename)))
                .Select(x => x.Split(','))
                .ToList();

            actualLines
                .Should()
                .BeEquivalentTo(csv);
        }
    }
}