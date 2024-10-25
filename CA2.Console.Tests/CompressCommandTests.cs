using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;

using AutoFixture;

using FluentAssertions.Execution;

using CA2.Compression;
using CA2.Extractors;

namespace CA2.Console.Tests;

public sealed class CompressCommandTests
{
    private readonly FixtureBuilder _builder = new();

    [Theory, AutoData]
    public async Task CcaFileGetsGenerated(int[] sizes, byte strength)
    {
        var fixture = _builder
            .WithRandomCsvFile(out var csvFilename)
            .Build();
        var ccaFilename = $"{Path.GetFileNameWithoutExtension(csvFilename)}.cca";

        var sut = fixture.Sut;
        await sut.Command(csvFilename, null, sizes, strength);

        fixture.AssetFileExists(ccaFilename);
    }

    [Theory, AutoData]
    public async Task CcaFileContainsExpectedContent(int[] sizes, byte strength)
    {
        var fixture = _builder
            .WithRandomCsvFile(out var csvFilename)
            .WithRandomCompressedCsv(out var expectedContent)
            .Build();

        var ccaFilename = $"{Path.GetFileNameWithoutExtension(csvFilename)}.cca";

        await fixture.Sut.Command(csvFilename, null, sizes, strength);

        fixture.AssetFileExists(ccaFilename, expectedContent);
    }

    [Theory, AutoData]
    public async Task RightCsvGotExtracted(int[] sizes, byte strength)
    {
        var fixture = _builder
            .WithRandomCsvFile(out var csvFilename, out _)
            .WithRandomOptimizedCsv(out var optimizedCsv)
            .WithRandomCompressedCsv()
            .Build();

        await fixture.Sut.Command(csvFilename, null, sizes, strength);

        using (new AssertionScope())
        {
            fixture.AssertRightContentWasCompressed(optimizedCsv);
            fixture.AssertRightSizesWereUsed(sizes);
        }
    }

    [Theory, AutoData]
    public async Task RightCsvGotCompressed(int[] sizes, byte strength)
    {
        var fixture = _builder
            .WithRandomCsvFile(out var csvFilename, out var csv)
            .WithRandomOptimizedCsv(out _)
            .WithRandomCompressedCsv()
            .Build();

        await fixture.Sut.Command(csvFilename, null, sizes, strength);

        using (new AssertionScope())
        {
            fixture.AssertRightContentWasExtracted(csv);
        }
    }

    private sealed class FixtureBuilder
    {
        private readonly IFixture _fixture = new AutoFixture.Fixture();
        private readonly Dictionary<string, MockFileData> _files = [];
        private readonly SpyCompressor _compressor = new();
        private SpyExtractor _extractor = new([]);

        public Fixture Build()
        {
            var fileSystem = new MockFileSystem(_files);

            var sut = new CompressCommand(
                fileSystem,
                _extractor,
                _compressor);

            return new Fixture(
                sut,
                fileSystem,
                _compressor,
                _extractor);
        }

        public FixtureBuilder WithRandomCsvFile(out string csvFilename)
            => WithRandomCsvFile(out csvFilename, out _);

        public FixtureBuilder WithRandomCsvFile(
            out string filename,
            out string csv)
        {
            filename = $"{_fixture.Create<string>()}.csv";
            csv = _fixture.Create<string>();

            _files[filename] = new MockFileData(csv);

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

        public FixtureBuilder WithRandomOptimizedCsv(out int[][] optimizedCsv)
        {
            optimizedCsv = _fixture.Create<int[][]>();

            _extractor = new SpyExtractor(optimizedCsv);

            return this;
        }
    }

    private sealed class Fixture(
        CompressCommand sut,
        IFileSystem fileSystem,
        SpyCompressor compressor,
        SpyExtractor extractor)
    {
        public CompressCommand Sut { get; } = sut;

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

        public void AssertRightContentWasExtracted(string csv)
            => extractor.CompressedCsvFiles.Should().ContainEquivalentOf(csv);

        public void AssertRightSizesWereUsed(int[] sizes)
            => compressor.Sizes.Should().ContainEquivalentOf(sizes);

        public void AssertRightContentWasCompressed(int[][] optimizedCsv) 
            => compressor.CompressedCsvFiles.Should().ContainEquivalentOf(optimizedCsv);
    }

    private sealed class SpyExtractor(int[][] optimizedCsv) : IExtractor
    {
        public List<string> CompressedCsvFiles { get; } = [];

        public Task<int[][]> ExtractAsync(TextReader reader)
        {
            var text = reader.ReadToEnd();

            CompressedCsvFiles.Add(text);

            return Task.FromResult(optimizedCsv);
        }
    }

    private sealed class SpyCompressor : ICompressor
    {
        private byte[] _result = [];

        public List<int[][]> CompressedCsvFiles { get; } = [];

        public List<int[]> Sizes { get; } = [];

        public void WithCompressionResult(byte[] result)
            => _result = result;

        public Task CompressAsync(
            int[][] csv,
            int[] sizes,
            byte interactionStrength,
            Stream ccaStream,
            Stream metaStream)
        {
            CompressedCsvFiles.Add(csv);
            Sizes.Add(sizes);

            ccaStream.Write(_result);

            return Task.CompletedTask;
        }
    }
}