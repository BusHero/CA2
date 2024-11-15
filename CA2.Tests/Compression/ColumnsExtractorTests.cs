namespace CA2.Tests.Compression;

public sealed class ColumnsExtractorTests
{
    [Property]
    public Property NumberBetween0x00And0x7fHasLength1()
    {
        var arb = Gen.Choose(0x00, 0x7f).ToArbitrary();

        return Prop.ForAll(arb, x =>
        {
            var bytes = ColumnsExtractor.GetBytes(x);

            return bytes.Length == 1;
        });
    }

    [Property]
    public Property NumberBetween0x00And0x7fHasValue()
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
    public Property NumberBetween0x80And0x3fffHasLength2()
    {
        var arb = Gen.Choose(0x80, 0x3fff).ToArbitrary();

        return Prop.ForAll(arb, x =>
        {
            var bytes = ColumnsExtractor.GetBytes(x);

            return bytes.Length == 2;
        });
    }

    [Property]
    public Property NumberBetween0x80And0x3fffFirstByteHasMask10()
    {
        var arb = Gen.Choose(0x80, 0x3fff).ToArbitrary();

        return Prop.ForAll(arb, x =>
        {
            var bytes = ColumnsExtractor.GetBytes(x);

            return (bytes[0] & 0xc0) == 0x80;
        });
    }

    [Property]
    public Property NumberBetween0x80And0x3fffFirstByteHasValue()
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
    public Property NumberBetween0x4000And0x1fffffHasLength3()
    {
        var arb = Gen.Choose(0x4000, 0x1fffff).ToArbitrary();

        return Prop.ForAll(arb, x =>
        {
            var bytes = ColumnsExtractor.GetBytes(x);

            return (bytes.Length == 3).Label($"length = {bytes.Length}");
        });
    }

    [Property]
    public Property NumberBetween0x4000And0x1fffffHasMask110()
    {
        var arb = Gen.Choose(0x4000, 0x1fffff).ToArbitrary();

        return Prop.ForAll(arb, x =>
        {
            var bytes = ColumnsExtractor.GetBytes(x);

            return ((bytes[0] & 0xc0) == 0xc0).Label($"{bytes[0]:b8} == {0xc0:b8}");
        });
    }

    [Property]
    public Property NumberBetween0x4000And0x1fffffHasValue()
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
    public Property NumberBetween0x2fffffAnd0x0fffffffHasLength4()
    {
        var arb = Gen.Choose(0x2fffff, 0x0fffffff).ToArbitrary();

        return Prop.ForAll(arb, x =>
        {
            var bytes = ColumnsExtractor.GetBytes(x);

            return (bytes.Length == 4).Label($"length = {bytes.Length}");
        });
    }

    [Property]
    public Property NumberBetween0x2fffffAnd0x0fffffffHasMask110()
    {
        var arb = Gen.Choose(0x2fffff, 0x0fffffff).ToArbitrary();

        return Prop.ForAll(arb, x =>
        {
            var bytes = ColumnsExtractor.GetBytes(x);

            return ((bytes[0] & 0xf0) == 0xe0).Label($"{bytes[0]:b8} == {0xe0:b8}");
        });
    }

    [Property]
    public Property NumberBetween0x2fffffAnd0x0fffffffHasValue()
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

            var expected = lengths
                .Zip(values, (length, value) => (value, length))
                .OrderBy(x => x.value)
                .ToArray();

            var columns = ColumnsExtractor.GetColumns(bytes);

            return columns.All(values.Contains);
        });

    [Property(EndSize = 10)]
    public Property MultiplePairs2(PositiveInt l) => Prop.ForAll(
        GetColumnsCountGen(0x01, 0xff_ff, l.Get),
        GetValuesGen(l.Get),
        (lenghts, values) =>
        {
            var columns = GetColumns(lenghts, values);

            var expected = lenghts
                .Zip(values, (length, value) => (value, length))
                .OrderBy(x => x.value)
                .ToArray();

            var bytes = ColumnsExtractor.GetColumns(columns);

            return lenghts
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


    private static byte[] GetColumns(int[] lenghts, byte[] values) => lenghts
        .Zip(values, (length, value) => ColumnsExtractor.GetBytes(length).Append(value))
        .SelectMany(x => x)
        .ToArray();
}
