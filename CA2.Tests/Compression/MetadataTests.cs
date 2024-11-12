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

    [Property]
    public Property MetadataColumnFrom1To127()
    {
        var arb = Gen
            .Elements(2, byte.MaxValue)
            .Zip(
                Gen.Elements(0b00000001, 0b01111111),
                (elementsPerColumn, nbrOfColumns) => Enumerable
                    .Repeat(elementsPerColumn, nbrOfColumns)
                    .ToArray())
            .Zip(Arb.Default.PositiveInt().Generator, (columns, rows) => (columns, rows: rows.Get))
            .Zip(Arb.Default.Byte().Generator, (foo, strength) => (foo.columns, foo.rows, strength))
            .ToArbitrary();

        return Prop.ForAll(arb, t =>
        {
            using var metaStream = new MemoryStream();

            _compressor
                .CompressAsync(
                    GetCsv(t.rows, t.columns),
                    t.columns,
                    t.strength,
                    Stream.Null,
                    metaStream)
                .Wait();

            var bytes = metaStream.ToArray()[ParameterSizesRange];

            return (bytes[0] & 0b10000000) == 0;
        });
    }
    
    [Property]
    public Property MetadataColumnFrom128To()
    {
        var arb = Gen
            .Elements(2, byte.MaxValue)
            .Zip(
                Gen.Elements(0b00000001, 0b01111111),
                (elementsPerColumn, nbrOfColumns) => Enumerable
                    .Repeat(elementsPerColumn, nbrOfColumns)
                    .ToArray())
            .Zip(Arb.Default.PositiveInt().Generator, (columns, rows) => (columns, rows: rows.Get))
            .Zip(Arb.Default.Byte().Generator, (foo, strength) => (foo.columns, foo.rows, strength))
            .ToArbitrary();

        return Prop.ForAll(arb, t =>
        {
            using var metaStream = new MemoryStream();

            _compressor
                .CompressAsync(
                    GetCsv(t.rows, t.columns),
                    t.columns,
                    t.strength,
                    Stream.Null,
                    metaStream)
                .Wait();

            var bytes = metaStream.ToArray()[ParameterSizesRange];

            return (bytes[0] & 0b10000000) == 0;
        });
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
        => GetCsv(rows.Get, realSizes);

    private static int[][] GetCsv(
        int rows,
        int[] realSizes)
        => new DefaultRandomCsvGeneratorFactory()
            .Create()
            .WithColumns(realSizes)
            .WithRowsCount(rows)
            .Generate()
            .Select(row => row.Select(int.Parse).ToArray())
            .ToArray();
}