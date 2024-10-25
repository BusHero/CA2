namespace CA2.Tests.Compression;

using System.Text;
using System.Threading.Tasks;

using CsvGenerator;

using CA2.Compression;

public sealed class MetadataTests
{
    private readonly Compressor _compressor = new();

    private readonly Range MAGIC_BYTES_RANGE = 0..4;
    private readonly Range CA_VERSION_RANGE = 4..6;
    private readonly Range NUMBER_OF_ROWS_RANGE = 6..14;
    private readonly Range INTERACTION_STRENGTH_RANGE = 14..15;
    private readonly Range PARAMETER_SIZES_RANGE = 15..^2;

    [Theory, AutoData]
    public async Task MetadataStreamContainsMagicBytes(
        NonEmptyArray<PositiveInt> sizes,
        PositiveInt rows,
        byte interactionStrength)
    {
        await using var metaStream = new MemoryStream();

        await _compressor.CompressAsync(
            GetCsv(rows, sizes),
            GetRealColumns(sizes),
            interactionStrength,
            Stream.Null,
            metaStream);

        var bytes = metaStream.ToArray();

        bytes[MAGIC_BYTES_RANGE]
           .Should()
           .BeEquivalentTo(Encoding.ASCII.GetBytes(" CCA"), x => x.WithStrictOrdering());
    }

    [Theory, AutoData]
    public async Task MetadataStreamContainsVersion(
        NonEmptyArray<PositiveInt> sizes,
        PositiveInt rows,
        byte interactionStrength)
    {
        await using var metaStream = new MemoryStream();

        await _compressor.CompressAsync(
            GetCsv(rows, sizes),
            GetRealColumns(sizes),
            interactionStrength,
            Stream.Null,
            metaStream);

        var bytes = metaStream.ToArray();

        var version = BitConverter.ToUInt16(bytes[CA_VERSION_RANGE]);
        version.Should().Be(2);
    }

    [Theory, AutoData]
    public async Task MetadataStreamContainsNumberOfRows(
        NonEmptyArray<PositiveInt> sizes,
        PositiveInt rows,
        byte interactionStrength)
    {
        await using var metaStream = new MemoryStream();

        await _compressor.CompressAsync(
            GetCsv(rows, sizes),
            GetRealColumns(sizes),
            interactionStrength,
            Stream.Null,
            metaStream);

        var bytes = metaStream.ToArray();

        var numberOfRows = BitConverter.ToInt64(bytes[NUMBER_OF_ROWS_RANGE]);
        numberOfRows.Should().Be(rows.Get);
    }

    [Theory, AutoData]
    public async Task MetadataStreamContainsInteractionStrength(
        NonEmptyArray<PositiveInt> sizes, 
        PositiveInt rows,
        byte interactionStrength)
    {
        await using var metaStream = new MemoryStream();
        var realColumns = GetRealColumns(sizes);
        await _compressor.CompressAsync(
            GetCsv(rows, sizes),
            realColumns,
            interactionStrength,
            Stream.Null,
            metaStream);

        var bytes = metaStream.ToArray();

        bytes[INTERACTION_STRENGTH_RANGE][0]
            .Should()
            .Be(interactionStrength);
    }

    [Theory, AutoData]
    public async Task MetadataContainsParameterSizes(
        NonEmptyArray<PositiveInt> sizes, 
        PositiveInt rows,
        byte interactionStrength)
    {
        await using var metaStream = new MemoryStream();
        var realColumns = GetRealColumns(sizes);
        await _compressor.CompressAsync(
            GetCsv(rows, sizes),
            realColumns,
            interactionStrength,
            Stream.Null,
            metaStream);

        var bytes = metaStream.ToArray()[PARAMETER_SIZES_RANGE];
        bytes.Should().HaveCount(realColumns.Length * 2);
    }

    private static int[] GetRealColumns(NonEmptyArray<PositiveInt> values)
        => values
            .Get
            .Select(x => x.Get)
            .Select(x => 2 < x ? x : 2)
            .ToArray();

    private static int[][] GetCsv(
        PositiveInt rows,
        NonEmptyArray<PositiveInt> sizes)
        => GetCsv(rows, GetRealColumns(sizes));

    private static int[][] GetCsv(
        PositiveInt rows,
        int[] realSizes)
        => new DefaultRandomCsvGeneratorFactory()
            .Create()
            .WithColumns(realSizes)
            .WithRowsCount(rows.Get)
            .Generate()
            .Select(row => row.Select(x => int.Parse(x)).ToArray())
            .ToArray();
}