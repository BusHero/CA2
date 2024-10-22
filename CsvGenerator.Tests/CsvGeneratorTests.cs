using AutoFixture;

namespace CsvGenerator.Tests;

public sealed class CsvGeneratorTests
{
    private readonly FixtureBuilder _builder
        = FixtureBuilder.CreateDefaultBuilder();

    [Theory, AutoData]
    public async Task Generate_AddToStream(
        int rowsCount,
        string[][] columns,
        string[][] csv)
    {
        using var stream = new MemoryStream();

        var fixture = _builder
            .WithRandomCsv(csv)
            .Build();

        await fixture.Sut.GenerateAsync(stream, rowsCount, columns);

        await fixture.AssetGeneratedCsvContainsExpectedCsv(stream, csv);
    }

    private class FixtureBuilder
    {
        public static FixtureBuilder CreateDefaultBuilder()
            => new FixtureBuilder()
                .WithRandomCsv();

        private readonly SpyCsvGenerator _csvGenerator = new();
        private readonly IFixture _fixture;

        private FixtureBuilder()
            => _fixture = new AutoFixture.Fixture();

        public Fixture Build()
        {
            var csvGeneratorFactory = Substitute.For<IRandomCsvGeneratorFactory>();
            csvGeneratorFactory.Create().Returns(_csvGenerator);

            var sut = new CsvGenerator(csvGeneratorFactory);

            return new Fixture(sut);
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
        CsvGenerator sut)
    {
        public CsvGenerator Sut { get; } = sut;

        public async Task AssetGeneratedCsvContainsExpectedCsv(
            Stream stream,
            string[][] csv)
        {
            stream.Position = 0;

            var lines = await GetLines(stream).ToListAsync();

            lines.Should().BeEquivalentTo(csv);
        }

        private static async IAsyncEnumerable<string[]> GetLines(Stream stream)
        {
            var reader = new StreamReader(stream);

            while (true)
            {
                var line = await reader.ReadLineAsync();

                if (line is null)
                {
                    break;
                }

                yield return line.Split(",");
            }
        }
    }
}