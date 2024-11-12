namespace CA2.Tests.Compression;

using System.Buffers.Binary;
using System.Collections.ObjectModel;

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
                t.columns,
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
                t.columns,
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
                t.columns,
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
                t.columns,
                t.strength,
                metaStream);

            var bytes = metaStream.ToArray();

            return bytes[InteractionStrengthRange][0] == t.strength;
        });
    }

    [Property]
    public Property MetadataColumnFrom0x01To0x7e()
    {
        var elementsPerColumnGen = Gen.Choose(0x02, 0xff);
        var numberOfColumnsGen = Gen.Choose(0x01, 0x7f);

        var columnsGen = elementsPerColumnGen
            .Zip(numberOfColumnsGen, Enumerable.Repeat)
            .Select(Enumerable.ToArray);

        var rowsGen = Arb.Default.PositiveInt().Generator.Select(x => x.Get);
        var strengthGen = Gen.Choose(0x02, 0xff).Select(Convert.ToByte);

        var arb = columnsGen
            .Zip(rowsGen, strengthGen, (columns, rows, strength) => (columns, rows, strength))
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
        var elementsPerColumnGen = Gen.Choose(0x02, 0xff);
        var numberOfColumnsGen = Gen.Choose(0x80, 0x3fff);

        var columnsGen = elementsPerColumnGen
            .Zip(numberOfColumnsGen, Enumerable.Repeat)
            .Select(Enumerable.ToArray);

        var rowsGen = Arb.Default.PositiveInt().Generator.Select(x => x.Get);
        var strengthGen = Gen.Choose(0x02, 0xff).Select(Convert.ToByte);

        var arb = columnsGen
            .Zip(rowsGen, strengthGen, (columns, rows, strength) => (columns, rows, strength))
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
            var length = BinaryPrimitives.ReadInt16BigEndian(bytesForLength) & 0x3fff;

            return Enumerable.Repeat((int)bytes[^1], length).SequenceEqual(t.columns);
        });
    }

    [Property]
    public Property MetadataColumnFrom0x01To0x7fContainRightMask()
    {
        var valuesGen = Gen.Choose(0x02, 0xff);
        var numberOfColumnsGen = Gen.Choose(0x01, 0x7f);
        var columnsGen = valuesGen
            .Zip(numberOfColumnsGen, Enumerable.Repeat)
            .Select(Enumerable.ToArray);

        var rowsGen = Arb.Default.PositiveInt().Generator.Select(x => x.Get);
        var strengthGen = Gen.Choose(0x02, 0xff).Select(Convert.ToByte);

        var arb = columnsGen
            .Zip(rowsGen, strengthGen, (columns, rows, strength) => (columns, rows, strength))
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

            return (bytes[0] & 0x80) == 0;
        });
    }

    private static Arbitrary<(ReadOnlyCollection<int> columns, int rows, byte strength)> GetMetadataInputParameters()
    {
        var columnGen = Gen.Choose(2, 20000)
            .NonEmptyListOf()
            .Select(CollectionExtensions.AsReadOnly);
        var rowsGen = Arb.Default.PositiveInt().Generator.Select(x => x.Get);
        var strengthGen = Gen.Choose(2, 0xff).Select(Convert.ToByte);

        var arb = columnGen
            .Zip(rowsGen, strengthGen, (columns, rows, strength) => (columns, rows, strength))
            .ToArbitrary();

        return arb;
    }
}