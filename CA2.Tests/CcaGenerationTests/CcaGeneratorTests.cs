namespace CA2.Tests.CcaGenerationTests;

using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;

using AutoFixture;

using GeneratorLibrary;

public sealed class CcaGeneratorTests
{
    private readonly FixtureBuilder _builder = new();

    [Fact]
    public async Task CcaFileGetsGenerated()
    {
        var fixture = _builder
            .WithRandomCsvFile(out var csvFilename)
            .Build();
        var ccaFilename = $"{Path.GetFileNameWithoutExtension(csvFilename)}.cca";

        var sut = fixture.Sut;
        await sut.GenerateCcaFile(csvFilename);

        fixture.AssetFileExists(ccaFilename);
    }

    [Fact]
    public async Task CcaFileContainsExpectedContent()
    {
        var fixture = _builder
            .WithRandomCsvFile(out var csvFilename)
            .WithRandomCompressedCsv(out var expectedContent)
            .Build();
        
        var ccaFilename = $"{Path.GetFileNameWithoutExtension(csvFilename)}.cca";

        await fixture.Sut.GenerateCcaFile(csvFilename);

        fixture.AssetFileExists(ccaFilename, expectedContent);
    }

    [Fact]
    public async Task RightCsvGotCompressed()
    {
        var fixture = _builder
            .WithRandomCsvFile(out var csvFilename, out var csv)
            .WithRandomCompressedCsv()
            .Build();

        await fixture.Sut.GenerateCcaFile(csvFilename);

        fixture.AssertRightCsvWasCompressed(csv);
    }

    private sealed class FixtureBuilder
    {
        private readonly IFixture _fixture = new AutoFixture.Fixture();
        private readonly Dictionary<string, MockFileData> _files = [];
        private readonly SpyCompressor _compressor = new();

        public Fixture Build()
        {
            var fileSystem = new MockFileSystem(_files);

            var sut = new CcaGenerator(fileSystem, _compressor);

            return new Fixture(
                sut,
                fileSystem,
                _compressor);
        }

        public FixtureBuilder WithRandomCsvFile(out string csvFilename)
            => WithRandomCsvFile(out csvFilename, out _);

        public FixtureBuilder WithRandomCsvFile(
            out string filename,
            out string[][] csv)
        {
            filename = $"{_fixture.Create<string>()}.csv";
            csv = _fixture.Create<string[][]>();
            var content = csv.Select(x => string.Join(",", x));

            _files[filename] = new MockFileData(string.Join(Environment.NewLine, content));

            return this;
        }

        public FixtureBuilder WithRandomCompressedCsv()
            => WithRandomCompressedCsv(out _);

        public FixtureBuilder WithRandomCompressedCsv(out byte[] bytes)
        {
            bytes = _fixture.Create<byte[]>();

            _compressor.WithCompressionResult(bytes);

            return this;
        }
    }

    private sealed class Fixture(
        CcaGenerator sut,
        IFileSystem fileSystem,
        SpyCompressor compressor)
    {
        public CcaGenerator Sut { get; } = sut;

        public void AssetFileExists(string ccaFilename, byte[] expectedContent)
        {
            AssetFileExists(ccaFilename);

            fileSystem.File
                .ReadAllBytes(ccaFilename)
                .Should()
                .BeEquivalentTo(
                    expectedContent,
                    options => options.WithStrictOrdering());
        }

        public void AssetFileExists(string filename)
            => fileSystem.File.Exists(filename).Should().BeTrue();

        public void AssertRightCsvWasCompressed(string[][] csv) 
            => compressor.CompressedCsvFiles.Should().ContainEquivalentOf(csv);
    }
    
    private sealed class SpyCompressor: ICsvCompressor
    {
        private byte[] _result = [];

        public List<string[][]> CompressedCsvFiles { get; } = [];

        public void WithCompressionResult(byte[] result) 
            => _result = result;

        public async Task CompressAsync(
            string[][] csv, 
            Stream stream, 
            CancellationToken cancellationToken = default)
        {
            CompressedCsvFiles.Add(csv);
            
            await stream.WriteAsync(_result, cancellationToken);
        }
    }
}