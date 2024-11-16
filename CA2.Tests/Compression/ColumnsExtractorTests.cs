namespace CA2.Tests.Compression;

public sealed class ColumnsExtractorTests
{
    [Property]
    public Property NumberBetween0X00And0X7FHasLength1()
    {
        var arb = Gen.Choose(0x00, 0x7f).ToArbitrary();

        return Prop.ForAll(arb, x =>
        {
            var bytes = ColumnsExtractor.GetBytes(x);

            return bytes.Length == 1;
        });
    }

    [Property]
    public Property NumberBetween0X00And0X7FHasValue()
    {
        var arb = Gen.Choose(0x00, 0x7f).ToArbitrary();

        return Prop.ForAll(arb, x =>
        {
            var bytes = ColumnsExtractor.GetBytes(x);

            var result = ColumnsExtractor.GetValue(bytes);

            return result == x;
        });
    }

    [Property]
    public Property NumberBetween0X80And0X3FffHasLength2()
    {
        var arb = Gen.Choose(0x80, 0x3fff).ToArbitrary();

        return Prop.ForAll(arb, x =>
        {
            var bytes = ColumnsExtractor.GetBytes(x);

            return bytes.Length == 2;
        });
    }

    [Property]
    public Property NumberBetween0X80And0X3FffFirstByteHasMask10()
    {
        var arb = Gen.Choose(0x80, 0x3fff).ToArbitrary();

        return Prop.ForAll(arb, x =>
        {
            var bytes = ColumnsExtractor.GetBytes(x);

            return (bytes[0] & 0xc0) == 0x80;
        });
    }

    [Property]
    public Property NumberBetween0X80And0X3FffFirstByteHasValue()
    {
        var arb = Gen.Choose(0x80, 0x3fff).ToArbitrary();

        return Prop.ForAll(arb, x =>
        {
            var bytes = ColumnsExtractor.GetBytes(x);

            var result = ColumnsExtractor.GetValue(bytes);

            return result == x;
        });
    }


    [Property]
    public Property NumberBetween0X4000And0X1FffffHasLength3()
    {
        var arb = Gen.Choose(0x4000, 0x1fffff).ToArbitrary();

        return Prop.ForAll(arb, x =>
        {
            var bytes = ColumnsExtractor.GetBytes(x);

            return (bytes.Length == 3).Label($"length = {bytes.Length}");
        });
    }

    [Property]
    public Property NumberBetween0X4000And0X1FffffHasMask110()
    {
        var arb = Gen.Choose(0x4000, 0x1fffff).ToArbitrary();

        return Prop.ForAll(arb, x =>
        {
            var bytes = ColumnsExtractor.GetBytes(x);

            return ((bytes[0] & 0xc0) == 0xc0).Label($"{bytes[0]:b8} == {0xc0:b8}");
        });
    }

    [Property]
    public Property NumberBetween0X4000And0X1FffffHasValue()
    {
        var arb = Gen.Choose(0x4000, 0x1fffff).ToArbitrary();

        return Prop.ForAll(arb, x =>
        {
            var bytes = ColumnsExtractor.GetBytes(x);

            var result = ColumnsExtractor.GetValue(bytes);

            return result == x;
        });
    }


    [Property]
    public Property NumberBetween0X2FffffAnd0X0FffffffHasLength4()
    {
        var arb = Gen.Choose(0x2fffff, 0x0fffffff).ToArbitrary();

        return Prop.ForAll(arb, x =>
        {
            var bytes = ColumnsExtractor.GetBytes(x);

            return (bytes.Length == 4).Label($"length = {bytes.Length}");
        });
    }

    [Property]
    public Property NumberBetween0X2FffffAnd0X0FffffffHasMask110()
    {
        var arb = Gen.Choose(0x2fffff, 0x0fffffff).ToArbitrary();

        return Prop.ForAll(arb, x =>
        {
            var bytes = ColumnsExtractor.GetBytes(x);

            return ((bytes[0] & 0xf0) == 0xe0).Label($"{bytes[0]:b8} == {0xe0:b8}");
        });
    }

    [Property]
    public Property NumberBetween0X2FffffAnd0X0FffffffHasValue()
    {
        var arb = Gen.Choose(0x2fffff, 0x0fffffff).ToArbitrary();

        return Prop.ForAll(arb, x =>
        {
            var bytes = ColumnsExtractor.GetBytes(x);

            var result = ColumnsExtractor.GetValue(bytes);

            return result == x;
        });
    }

    [Property]
    public Property MultiplePairs(PositiveInt l) => Prop.ForAll(
        GetColumnsCountGen(0x01, 0xff_ff, l.Get),
        GetValuesGen(l.Get),
        (lengths, values) =>
        {
            var bytes = GetColumns(lengths, values);

            var columns = ColumnsExtractor.GetColumns(bytes);

            return columns.All(values.Contains);
        });

    [Property(EndSize = 10)]
    public Property MultiplePairs2(PositiveInt l) => Prop.ForAll(
        GetColumnsCountGen(0x01, 0xff_ff, l.Get),
        GetValuesGen(l.Get),
        (lengths, values) =>
        {
            var columns = GetColumns(lengths, values);

            var bytes = ColumnsExtractor.GetColumns(columns);

            return lengths
                .Zip(
                    values,
                    (length, value) => bytes.Count(x => x == value) == length)
                .All(x => x);
        });

    private static Arbitrary<int[]> GetColumnsCountGen(int l, int h, int length) => Gen
        .Choose(l, h)
        .ArrayOf(length)
        .ToArbitrary();

    private static Arbitrary<byte[]> GetValuesGen(int length) => Gen
        .Choose(0x02, 0xff)
        .Select(Convert.ToByte)
        .ArrayOf(length)
        .Where(x => x.ToHashSet().Count == x.Length)
        .ToArbitrary();


    private static byte[] GetColumns(int[] lengths, byte[] values) => lengths
        .Zip(values, (length, value) => ColumnsExtractor.GetBytes(length).Append(value))
        .SelectMany(x => x)
        .ToArray();
}
