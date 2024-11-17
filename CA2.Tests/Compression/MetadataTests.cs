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
    public Property MetadataStreamContainsMagicBytes() => Prop.ForAll(
        GetColumnGen(),
        GetRowsGen(),
        GetStrengthGen(),
        (columns, rows, strength) =>
        {
            using var metaStream = new MemoryStream();

            _compressor.WriteMetadata(
                rows,
                columns,
                strength,
                metaStream);

            var magicNumberBytes = metaStream.ToArray()[MagicBytesRange];

            return magicNumberBytes.SequenceEqual(" CCA"u8.ToArray());
        });

    [Property]
    public Property MetadataStreamContainsVersion() => Prop.ForAll(
        GetColumnGen(),
        GetRowsGen(),
        GetStrengthGen(),
        (columns, rows, strength) =>
        {
            using var metaStream = new MemoryStream();

            _compressor.WriteMetadata(
                rows,
                columns,
                strength,
                metaStream);

            var bytes = metaStream.ToArray();

            var version = BitConverter.ToUInt16(bytes[CaVersionRange]);

            return version == 2;
        });

    [Property]
    public Property MetadataStreamContainsNumberOfRows() => Prop.ForAll(
        GetColumnGen(),
        GetRowsGen(),
        GetStrengthGen(),
        (columns, rows, strength) =>
        {
            using var metaStream = new MemoryStream();

            _compressor.WriteMetadata(
                rows,
                columns,
                strength,
                metaStream);

            var bytes = metaStream.ToArray();

            var numberOfRows = BitConverter.ToInt64(bytes[NumberOfRowsRange]);

            return numberOfRows == rows;
        });

    [Property]
    public Property MetadataStreamContainsInteractionStrength() => Prop.ForAll(
        GetColumnGen(),
        GetRowsGen(),
        GetStrengthGen(),
        (columns, rows, strength) =>
        {
            using var metaStream = new MemoryStream();

            _compressor.WriteMetadata(
                rows,
                columns,
                strength,
                metaStream);

            var bytes = metaStream.ToArray();

            return bytes[InteractionStrengthRange][0] == strength;
        });

    [Property]
    public Property MetadataColumnFrom0X01To0X7EContainsAllColumns()
    {
        var columnsGen = GetColumnsWithSingleValue(0x01, 0x7f);
        var rowsGen = Arb.Default.PositiveInt().Generator.Select(x => x.Get).ToArbitrary();
        var strengthGen = Gen.Choose(0x02, 0xff).Select(Convert.ToByte).ToArbitrary();

        return Prop.ForAll(
            columnsGen,
            rowsGen,
            strengthGen,
            (columns, rows, strength) =>
            {
                using var metaStream = new MemoryStream();

                _compressor.WriteMetadata(
                    rows,
                    columns,
                    strength,
                    metaStream);

                var bytes = metaStream.ToArray()[ParameterSizesRange];

                var columns2 = ColumnsExtractor.GetColumns(bytes);

                return columns2.Order().SequenceEqual(columns2.Order());
            });
    }

    [Property]
    public Property MetadataColumnFrom0X01To0X7FContainRightMask()
        => Prop.ForAll(
            GetColumnsWithSingleValue(0x01, 0x7f),
            GetRowsGen(),
            GetStrengthGen(),
            MetadataContainsRightMasks);

    [Property]
    public Property MetadataColumnFrom0X80To0X3FffContainsRightMask()
        => Prop.ForAll(
            GetColumnsWithSingleValue(0x80, 0x3fff),
            GetRowsGen(),
            GetStrengthGen(),
            MetadataContainsRightMasks);

    [Property]
    public Property MetadataColumnFrom0X4FffTo0X1FffffContainsRightMask()
        => Prop.ForAll(
            GetColumnsWithSingleValue(0x4fff, 0x1fffff),
            GetRowsGen(),
            GetStrengthGen(),
            MetadataContainsRightMasks);

    [Property(MaxTest = 5)]
    public Property MetadataColumnFrom0X2FffTo0X0FffffffContainsRightMask()
        => Prop.ForAll(
            GetColumnsWithSingleValue(0x2fffff, 0x0fffffff),
            GetRowsGen(),
            GetStrengthGen(),
            MetadataContainsRightMasks);


    [Property]
    public Property MetadataColumnContainsRightMask()
        => Prop.ForAll(
            GetColumnGen(),
            GetRowsGen(),
            GetStrengthGen(),
            MetadataContainsRightMasks);

    [Property]
    public Property MetadataContainAllColumns2()
        => Prop.ForAll(
            GetColumnGen(),
            GetRowsGen(),
            GetStrengthGen(),
            MetadataContainAllColumns);

    private void MetadataContainsRightMasks(int[] columns, int rows, byte strength)
    {
        using var metaStream = new MemoryStream();

        _compressor.WriteMetadata(
            rows,
            columns,
            strength,
            metaStream);
        var bytes = metaStream.ToArray()[ParameterSizesRange];

        var initial = 0;
        while (initial != bytes.Length)
        {
            //GetNumberOfBytesFromMask will fail if a byte does not contain the right mask
            var numberOfBytes = ColumnsExtractor.GetNumberOfBytesFromMask(bytes[initial]);
            initial += numberOfBytes + 1;
        }
    }

    private void MetadataContainAllColumns(int[] columns, int rows, byte strength)
    {
        using var metaStream = new MemoryStream();

        _compressor.WriteMetadata(
            rows,
            columns,
            strength,
            metaStream);

        var bytes = metaStream.ToArray()[ParameterSizesRange];

        var actualColumns = ColumnsExtractor.GetColumns(bytes);

        actualColumns.Should().BeEquivalentTo(columns.OrderDescending(), x => x.WithStrictOrdering());
    }

    private static Arbitrary<int[]> GetColumnGen() => Gen
        .Choose(2, 0xff)
        .NonEmptyListOf()
        .Select(Enumerable.ToArray)
        .ToArbitrary();

    private static Arbitrary<int> GetRowsGen() => Arb.Default
        .PositiveInt()
        .Generator
        .Select(x => x.Get)
        .ToArbitrary();

    private static Arbitrary<byte> GetStrengthGen() => Gen
        .Choose(2, 0xff)
        .Select(Convert.ToByte)
        .ToArbitrary();

    private static Arbitrary<int[]> GetColumnsWithSingleValue(int l, int h)
    {
        var elementsPerColumnGen = Gen.Choose(0x02, 0xff);
        var numberOfColumnsGen = Gen.Choose(l, h);

        var columnsGen = elementsPerColumnGen
            .Zip(numberOfColumnsGen, Enumerable.Repeat)
            .Select(Enumerable.ToArray).ToArbitrary();
        return columnsGen;
    }
}