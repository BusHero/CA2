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
    public Property OnePairOfLengthPlusValue()
    {
        var columnsCountGen = Gen.Choose(0x01, 0x0f_ff_ff_ff);
        var valueGen = Gen.Choose(0x02, 0xff).Select(Convert.ToByte);

        var arb = columnsCountGen.Zip(valueGen, (length, values) => (length, bytes: ColumnsExtractor.GetBytes(length).Append(values).ToArray()))
            .ToArbitrary();

        return Prop.ForAll(arb, t =>
        {
            var columns = ColumnsExtractor.Extract(t.bytes);

            return (columns.Length == t.length).Label($"{columns.Length} == {t.length}");
        });
    }

    [Property]
    public Property MultiplePairs(PositiveInt l)
    {
        var columnsCountGen = Gen.Sized(size => Gen.Choose(0x01, 0xff_ff).Select(x => x % size)).ArrayOf(l.Get);
        var valueGen = Gen.Choose(0x02, 0xff).Select(Convert.ToByte).ArrayOf(l.Get);

        var arb = columnsCountGen.Zip(
            valueGen,
            (lenghts, values) =>
            {
                var bytes = lenghts
                    .Zip(values, (length, value) => ColumnsExtractor.GetBytes(length).Append(value))
                    .SelectMany(x => x)
                    .ToArray();

                return (lenghts, values, bytes);
            })
            .ToArbitrary();

        return Prop.ForAll(arb, t =>
        {
            var expected = t.lenghts
                .Zip(t.values, (length, value) => (value, length))
                .OrderBy(x => x.value)
                .ToArray();

            var bytes = ColumnsExtractor.Extract(t.bytes);

            return bytes.All(t.values.Contains);
        });
    }

    [Property]
    public Property MultiplePairs2(PositiveInt l)
    {
        var columnsCountGen = Gen
            .Choose(0x01, 0xff_ff)
            .ArrayOf(l.Get);
        var valueGen = Gen.Choose(0x02, 0xff).Select(Convert.ToByte).ArrayOf(l.Get)
            .Where(x => x.Distinct().Count() == x.Length);
            ;

        var arb = columnsCountGen.Zip(
            valueGen,
            (lenghts, values) =>
            {
                var bytes = lenghts
                    .Zip(values, (length, value) => ColumnsExtractor.GetBytes(length).Append(value))
                    .SelectMany(x => x)
                    .ToArray();

                return (lenghts, values, bytes);
            })
            .ToArbitrary();

        return Prop.ForAll(arb, t =>
        {
            var expected = t.lenghts
                .Zip(t.values, (length, value) => (value, length))
                .OrderBy(x => x.value)
                .ToArray();

            var bytes = ColumnsExtractor.Extract(t.bytes);

            var result = t.lenghts
                .Zip(
                    t.values, 
                    (length, value) => bytes.Count(x => x == value) == length)
                .All(x => x);

            return result;
        });
    }
}

public static class ColumnsExtractor
{
    internal static byte[] Extract(byte[] bytes)
    {
        var result = Enumerable.Empty<byte>();

        while (bytes.Length != 0)
        {
            var length = GetValue(bytes);
            var numberOfBytes = CountBytes(length);

            var sequence = Enumerable.Repeat(bytes[numberOfBytes], length);
            result = result.Concat(sequence);
            bytes = bytes[(numberOfBytes + 1)..];
        }

        return result.ToArray();
    }

    private static int CountBytes(int x) => x switch
    {
        <= 0x7f => 1,
        <= 0x3fff => 2,
        <= 0x1fffff => 3,
        <= 0x0fffffff => 4,
        _ => throw new InvalidOperationException(),
    };

    internal static byte[] GetBytes(int x)
        => x switch
        {
            <= 0x7f => [(byte)x],
            <= 0x3fff => [(byte)(x >> 8 | 0x80), (byte)(x & 0xff),],
            <= 0x1fffff => [(byte)(x >> 16 | 0xc0), (byte)(x >> 8 & 0xff), (byte)(x & 0xff),],
            <= 0x0fffffff => [(byte)(x >> 24 | 0xe0), (byte)(x >> 16 & 0xff), (byte)(x >> 8 & 0xff), (byte)(x & 0xff),],
            _ => throw new InvalidOperationException(),
        };

    internal static int GetValue(byte[] bytes)
        => bytes[0] switch
        {
            var b when (b & 0x80) == 0x00 => bytes[0],
            var b when (b & 0xc0) == 0x80 => ((bytes[0] & 0x7f) << 8) | bytes[1],
            var b when (b & 0xe0) == 0xc0 => ((bytes[0] & 0x3f) << 16) | (bytes[1] << 8) | bytes[2],
            var b when (b & 0xf0) == 0xe0 => ((bytes[0] & 0x0f) << 24) | (bytes[1] << 16) | (bytes[2] << 8) | bytes[3],
            _ => throw new InvalidOperationException(),
        };
}
