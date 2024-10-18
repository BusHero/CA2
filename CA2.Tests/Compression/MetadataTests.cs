namespace CA2.Tests.Compression;

using System.Text;
using System.Threading.Tasks;

using CsvGenerator;

using GeneratorLibrary.Compression;

public sealed class MetadataTests
{
    private readonly Compressor _compressor = new();

    private readonly Range MAGIC_BYTES_RANGE = 0..4;
    private readonly Range CA_VERSION_RANGE = 4..5;

    [Theory, AutoData]
    public async Task MetadataStreamContainsMagicBytes(NonEmptyArray<PositiveInt> sizes, PositiveInt rows)
    {
        await using var metaStream = new MemoryStream();

        await _compressor.CompressAsync(
            GetCsv(rows, sizes),
            GetRealColumns(sizes),
            Stream.Null,
            metaStream);

        var bytes = metaStream.ToArray();

        bytes[MAGIC_BYTES_RANGE]
           .Should()
           .BeEquivalentTo(Encoding.ASCII.GetBytes(" CCA"), x => x.WithStrictOrdering());
    }

    [Theory, AutoData]
    public async Task MetadataStreamContainsVersion(NonEmptyArray<PositiveInt> sizes, PositiveInt rows)
    {
        await using var metaStream = new MemoryStream();

        await _compressor.CompressAsync(
            GetCsv(rows, sizes),
            GetRealColumns(sizes),
            Stream.Null,
            metaStream);

        var bytes = metaStream.ToArray();

        bytes[CA_VERSION_RANGE]
           .Should()
           .BeEquivalentTo([0x2], x => x.WithStrictOrdering());
    }

    private static int[] GetRealColumns(NonEmptyArray<PositiveInt> values)
        => values
            .Get
            .Select(x => x.Get)
            .Select(x => 2 < x ? x : 2)
            .ToArray();

    private static string[][] GetCsv(
        PositiveInt rows,
        NonEmptyArray<PositiveInt> sizes)
        => GetCsv(rows, GetRealColumns(sizes));

    private static string[][] GetCsv(
        PositiveInt rows,
        int[] realSizes)
        => new RandomCsvGenerator()
            .WithColumns(realSizes)
            .WithRowsCount(rows.Get)
            .Generate();
}