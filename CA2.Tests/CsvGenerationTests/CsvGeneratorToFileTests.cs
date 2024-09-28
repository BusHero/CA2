namespace CA2.Tests.CsvGenerationTests;

using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;

using AutoFixture;

using FluentAssertions.Execution;

using GeneratorLibrary;

public class CsvGeneratorToFileTests
{
    private readonly FixtureBuilder _builder
        = FixtureBuilder.CreateDefaultBuilder();

    [Theory, AutoData]
    public async Task FileIsCreated(
        string filename,
        int rowsCount,
        string[][] columns)
    {
        var fixture = _builder
            .Build();

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
    public async Task Generate_ExpectedCsvIsGenerated(
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

    [Theory, AutoData]
    public async Task Generate_ExpectedRowsCount(
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

        fixture.AssertExpectedRowsCount(rowsCount);
    }

    [Theory, AutoData]
    public async Task Generate_ExpectedColumns(
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

        fixture.AssertExpectedColumns(columns);
    }

    private class FixtureBuilder
    {
        public static FixtureBuilder CreateDefaultBuilder()
        {
            var builder = new FixtureBuilder()
                .WithRandomCsv();

            return builder;
        }

        private readonly Dictionary<string, MockFileData> _fileData = [];
        private readonly SpyCsvGenerator _csvGenerator = new();
        private readonly IFixture _fixture;

        private FixtureBuilder()
            => _fixture = new AutoFixture.Fixture();

        public Fixture Build()
        {
            var fileSystem = new MockFileSystem(_fileData);

            var sut = new CsvGeneratorToFile(
                fileSystem,
                _csvGenerator);

            return new Fixture(
                sut,
                fileSystem,
                _csvGenerator);
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

        private FixtureBuilder WithRandomCsv()
        {
            var csv = _fixture
                .CreateMany<string[]>()
                .ToArray();

            return WithRandomCsv(csv);
        }

        public FixtureBuilder WithRandomCsv(string[][] csv)
        {
            _csvGenerator.WithRandomCsv(csv);

            return this;
        }
    }

    private class Fixture(
        CsvGeneratorToFile sut,
        IFileSystem fileSystem,
        SpyCsvGenerator csvGenerator)
    {
        public CsvGeneratorToFile Sut { get; } = sut;

        public void AssetFileExists(string parent, string filename)
            => AssetFileExists(Path.Combine(parent, filename));

        public void AssetFileExists(string filename)
            => fileSystem
                .File
                .Exists(filename)
                .Should()
                .BeTrue();

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

        public void AssertExpectedRowsCount(int rowsCount)
            => csvGenerator.RowsCount.Should().Be(rowsCount);

        public void AssertExpectedColumns(string[][] columns)
        {
            using (new AssertionScope())
            {
                csvGenerator
                    .Columns
                    .Should()
                    .HaveSameCount(columns);

                var actualColumns = csvGenerator
                    .Columns
                    .Select(x => x switch
                    {
                        SpyCsvGenerator.ValuesColumnDefinition { Values: var values } => values,
                        _ => default,
                    })
                    .ToArray();

                actualColumns
                    .Should()
                    .BeEquivalentTo(columns);
            }
        }
    }
}