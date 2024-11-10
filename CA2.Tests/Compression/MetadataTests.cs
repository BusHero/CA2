namespace CA2.Tests.Compression;

using System.Threading.Tasks;

using CA2.Compression;

using CsvGenerator;

public sealed class MetadataTests
{
    private readonly Compressor _compressor = new();

    private static readonly Range MagicBytesRange = ..4;
    private static readonly Range CaVersionRange = 4..6;
    private static readonly Range NumberOfRowsRange = 6..14;
    private static readonly Range InteractionStrengthRange = 14..15;
    private static readonly Range ParameterSizesRange = 15..^2;

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

        bytes[MagicBytesRange]
            .Should()
            .BeEquivalentTo(" CCA"u8.ToArray(), x => x.WithStrictOrdering());
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

        var version = BitConverter.ToUInt16(bytes[CaVersionRange]);
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

        var numberOfRows = BitConverter.ToInt64(bytes[NumberOfRowsRange]);
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

        bytes[InteractionStrengthRange][0]
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

        var bytes = metaStream.ToArray()[ParameterSizesRange];
        bytes.Should().HaveCount(realColumns.Length);
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
            .Select(row => row.Select(int.Parse).ToArray())
            .ToArray();
}