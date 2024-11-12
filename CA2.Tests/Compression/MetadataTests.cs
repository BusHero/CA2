namespace CA2.Tests.Compression;

using CA2.Compression;

public sealed class MetadataTests
{
    private readonly Compressor _compressor = new();

    private static readonly Range MagicBytesRange = ..4;
    private static readonly Range CaVersionRange = 4..6;
    private static readonly Range NumberOfRowsRange = 6..14;
    private static readonly Range InteractionStrengthRange = 14..15;
    private static readonly Range ParameterSizesRange = 15..^2;

    [Property]
    public Property MetadataStreamContainsMagicBytes()
    {
        return Prop.ForAll(GetMetadataInputParameters(), t =>
        {
            using var metaStream = new MemoryStream();

            _compressor.Write(
                t.rows,
                t.columns.AsReadOnly(),
                t.strength,
                metaStream);

            var magicNumberBytes = metaStream.ToArray()[MagicBytesRange];

            return magicNumberBytes.SequenceEqual(" CCA"u8.ToArray());
        });
    }

    [Property]
    public Property MetadataStreamContainsVersion()
    {
        return Prop.ForAll(GetMetadataInputParameters(), t =>
        {
            using var metaStream = new MemoryStream();

            _compressor.Write(
                t.rows,
                t.columns.AsReadOnly(),
                t.strength,
                metaStream);

            var bytes = metaStream.ToArray();

            var version = BitConverter.ToUInt16(bytes[CaVersionRange]);

            return version == 2;
        });

    }

    [Property]
    public Property MetadataStreamContainsNumberOfRows()
    {
        return Prop.ForAll(GetMetadataInputParameters(), t =>
        {
            using var metaStream = new MemoryStream();

            _compressor.Write(
                t.rows,
                t.columns.AsReadOnly(),
                t.strength,
                metaStream);

            var bytes = metaStream.ToArray();

            var numberOfRows = BitConverter.ToInt64(bytes[NumberOfRowsRange]);

            return numberOfRows == t.rows;
        });
    }

    [Property]
    public Property MetadataStreamContainsInteractionStrength()
    {
        return Prop.ForAll(GetMetadataInputParameters(), t =>
        {
            using var metaStream = new MemoryStream();

            _compressor.Write(
                t.rows,
                t.columns.AsReadOnly(),
                t.strength,
                metaStream);

            var bytes = metaStream.ToArray();

            return bytes[InteractionStrengthRange][0] == t.strength;
        });
    }

    [Property]
    public Property MetadataColumnFrom1To127()
    {
        var arb = Gen
            .Elements(Enumerable.Range(2, byte.MaxValue - 2))
            .Zip(
                Gen.Elements(Enumerable.Range(0x02, 0x7f - 0x02)),
                (elementsPerColumn, nbrOfColumns) => Enumerable
                    .Repeat(elementsPerColumn, nbrOfColumns)
                    .ToArray())
            .Zip(Arb.Default.PositiveInt().Generator, (columns, rows) => (columns, rows: rows.Get))
            .Zip(Arb.Default.Byte().Generator, (foo, strength) => (foo.columns, foo.rows, strength))
            .ToArbitrary();

        return Prop.ForAll(arb, t =>
        {
            using var metaStream = new MemoryStream();

            _compressor.Write(
                t.rows,
                t.columns,
                t.strength,
                metaStream);

            var bytes = metaStream.ToArray()[ParameterSizesRange];

            return Enumerable.Repeat((int)bytes[^1], bytes[0]).SequenceEqual(t.columns);
        });
    }

    [Property]
    public Property MetadataColumnFrom0x80To0x3fff()
    {
        var arb = Gen
            .Elements(Enumerable.Range(2, byte.MaxValue - 2))
            .Zip(
                Gen.Elements(Enumerable.Range(0x80, 0x3fff - 0x80)),
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
                .Write(
                    t.rows,
                    t.columns,
                    t.strength,
                    metaStream);

            var bytes = metaStream.ToArray()[ParameterSizesRange];
            var bytesForLength = bytes[..2];
            Array.Reverse(bytesForLength);

            var length = BitConverter.ToInt16(bytesForLength) & 0x3fff;

            return Enumerable.Repeat((int)bytes[^1], length).SequenceEqual(t.columns);
        });
    }

    [Property]
    public Property MetadataColumnFrom128To()
    {
        var arb = Gen
            .Elements(Enumerable.Range(2, byte.MaxValue - 2))
            .Zip(
                Gen.Elements(Enumerable.Range(0b00000001, 0b01111111 - 0b00000001)),
                (elementsPerColumn, nbrOfColumns) => Enumerable
                    .Repeat(elementsPerColumn, nbrOfColumns)
                    .ToArray())
            .Zip(Arb.Default.PositiveInt().Generator, (columns, rows) => (columns, rows: rows.Get))
            .Zip(Arb.Default.Byte().Generator, (foo, strength) => (foo.columns, foo.rows, strength))
            .ToArbitrary();

        return Prop.ForAll(arb, t =>
        {
            using var metaStream = new MemoryStream();

            _compressor .Write(
                t.rows,
                t.columns,
                t.strength,
                metaStream);

            var bytes = metaStream.ToArray()[ParameterSizesRange];

            return (bytes[0] & 0b10000000) == 0;
        });
    }

    private static Arbitrary<(IList<int> columns, int rows, byte strength)> GetMetadataInputParameters()
    {
        var columnGen = Gen.Elements(Enumerable.Range(2, 20000)).NonEmptyListOf();
        var rowsGen = Arb.Default.PositiveInt().Generator.Select(x => x.Get);
        var strengthGen = Gen.Elements(Enumerable.Range(2, byte.MaxValue - 2)).Select(x => (byte)x);

        var arb = columnGen
            .Zip(rowsGen, strengthGen, (columns, rows, strength) => (columns, rows, strength))
            .ToArbitrary();

        return arb;
    }
}