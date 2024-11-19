namespace CA2.Tests.Compression;

using System.Collections.ObjectModel;
using System.Numerics;

using CA2.Compression;

using CsvGenerator;

public sealed class CompressorTests
{
    private readonly Compressor _compressor = new();

    public static TheoryData<int[][], int[], int> Data => new()
    {
        { [[0, 0]], [2, 2], 0 },
        { [[0, 1]], [2, 2], 1 },
        { [[1, 0]], [2, 2], 2 },
        { [[1, 1]], [2, 2], 3 },

        { [[0, 0]], [3, 2], 0 },
        { [[0, 1]], [3, 2], 1 },
        { [[1, 0]], [3, 2], 2 },
        { [[1, 1]], [3, 2], 3 },
        { [[2, 0]], [3, 2], 4 },
        { [[2, 1]], [3, 2], 5 },

        { [[0, 0]], [2, 3], 0 },
        { [[0, 1]], [2, 3], 2 },
        { [[0, 2]], [2, 3], 4 },
        { [[1, 0]], [2, 3], 1 },
        { [[1, 1]], [2, 3], 3 },
        { [[1, 2]], [2, 3], 5 },

        { [[0, 0]], [3, 3], 0 },
        { [[0, 1]], [3, 3], 1 },
        { [[0, 2]], [3, 3], 2 },
        { [[1, 0]], [3, 3], 3 },
        { [[1, 1]], [3, 3], 4 },
        { [[1, 2]], [3, 3], 5 },
        { [[2, 0]], [3, 3], 6 },
        { [[2, 1]], [3, 3], 7 },
        { [[2, 2]], [3, 3], 8 },

        { [[0, 0, 0]], [2, 2, 2], 0 },
        { [[0, 0, 1]], [2, 2, 2], 1 },
        { [[0, 1, 0]], [2, 2, 2], 2 },
        { [[0, 1, 1]], [2, 2, 2], 3 },
        { [[1, 0, 0]], [2, 2, 2], 4 },
        { [[1, 0, 1]], [2, 2, 2], 5 },
        { [[1, 1, 0]], [2, 2, 2], 6 },
        { [[1, 1, 1]], [2, 2, 2], 7 },
        { [[1, 1, 0, 0]], [2, 2, 2, 2], 12 },
        { [[1, 2, 1, 1, 1, 0, 0]], [2, 7, 3, 2, 2, 2, 2], 252 },
        { [[1, 0, 0, 1, 0, 0, 1, 4, 1]], [2, 2, 2, 2, 2, 2, 2, 7, 2], 1_171 },
        { [[1, 2, 1]], [2, 3, 2], 11 },
    };

    [Theory]
    [MemberData(nameof(Data))]
    public async Task GenerateGeneratesExpectedNumber(
        int[][] item,
        int[] sizes,
        int expectedResult)
    {
        using var stream = await CompressAsync(item, sizes);

        GetBigNumber(stream)
            .Should()
            .Be(expectedResult);
    }

    [Property]
    public Property ResultShouldBeBetweenZeroAndMaxValue() => Prop.ForAll(
        Gen.OneOf(RandomCsv, RandomCsvWithZeros, RandomCsvWithMaxValuesPerColumn)
            .Select(x => x with { Values = [x.Values[0]] })
            .ToArbitrary(),
        csv =>
        {
            using var stream = CompressAsync(csv.Values, csv.Columns).Result;

            var result = GetBigNumber(stream);

            return 0 <= result && result < GetMaxPossibleNumber(csv);
        });

    [Property]
    public Property DifferentRowsGenerateDifferentValues() => Prop.ForAll(
        RandomCsv
            .Where(csv => csv is { Values.Length: > 2 })
            .Where(x => !x.Values[0].SequenceEqual(x.Values[1]))
            .Select(csv => (
                csv with { Values = [csv.Values[0]] },
                csv with { Values = [csv.Values[1]] }))
            .ToArbitrary(),
        t =>
        {
            using var stream1 = CompressAsync(t.Item1.Values, t.Item1.Columns).Result;
            using var stream2 = CompressAsync(t.Item2.Values, t.Item2.Columns).Result;

            return GetBigNumber(stream1) != GetBigNumber(stream2);
        });

    [Property]
    public Property SmallerCsvGeneratesSmallerNumberThanBiggerCsv() => Prop.ForAll(
        Gen.OneOf(CsvWithOneValueSmallerThanOther, CsvWithOneColumnBiggerThanAnother)
            .ToArbitrary(),
        t =>
        {
            using var smallerStream = CompressAsync(t.Smaller.Values, t.Smaller.Columns).Result;
            using var biggerStream = CompressAsync(t.Bigger.Values, t.Bigger.Columns).Result;

            var smaller = GetBigNumber(smallerStream);
            var bigger = GetBigNumber(biggerStream);

            return smaller <= bigger;
        });

    [Property]
    public Property KindOfIgnoresTheOrder() => Prop.ForAll(
        RandomCsv
            .Select(csv => csv with { Values = [csv.Values[0]] })
            .ToArbitrary(),
        csv =>
        {
            var stuff = csv
                .Values[0]
                .Zip(csv.Columns, (value, column) => (value, column))
                .OrderByDescending(x => x.column)
                .ToArray();

            var sortedCsv = new Csv(
                Values: [[..stuff.Select(x => x.value)]],
                Columns: [..stuff.Select(x => x.column)]);

            using var sortedStream = CompressAsync(csv.Values, csv.Columns).Result;
            using var originalStream = CompressAsync(sortedCsv.Values, sortedCsv.Columns).Result;

            var sortedResult = GetBigNumber(sortedStream);
            var originalResult = GetBigNumber(originalStream);

            return sortedResult == originalResult;
        });

    [Property]
    public Property CompressSortsColumnsInDescendingOrder() => Prop.ForAll(
        RandomCsv
            .Where(csv => csv is { Columns.Count: > 2 })
            .Where(csv => csv.Values[0].All(x => x != 0))
            .ToArbitrary(),
        csv =>
        {
            var columns = csv.Columns.ToArray();

            var maxIndex = Array.IndexOf(columns, columns.Max());
            var minIndex = Array.IndexOf(columns, columns.Min());

            var biggerCsvValues = csv.Values[0].ToArray();
            biggerCsvValues[maxIndex]--;

            var smallerCsvValues = csv.Values[0].ToArray();
            smallerCsvValues[minIndex]--;

            using var biggerStream = CompressAsync([biggerCsvValues], csv.Columns).Result;
            using var smallerStream = CompressAsync([smallerCsvValues], csv.Columns).Result;

            var biggerResult = GetBigNumber(biggerStream);
            var smallerResult = GetBigNumber(smallerStream);

            return biggerResult <= smallerResult;
        });

    [Property]
    public Property CompressingMultipleRowsUseMoreBytesThanCompressingASingleRow() => Prop.ForAll(
        RandomCsv.Where(csv => csv is { Values.Length: >= 2 }).ToArbitrary(),
        csv =>
        {
            csv = csv with { Values = csv.Values[0..2] };
            var csvWithSingleLine = csv with { Values = [csv.Values[0]] };

            using var singleRowStream = CompressAsync(csvWithSingleLine.Values, csvWithSingleLine.Columns).Result;
            using var multiRowStream = CompressAsync(csv.Values, csv.Columns).Result;

            return multiRowStream.Length == singleRowStream.Length * csv.Values.Length;
        });

    private static Gen<SmallerAndBiggerCsv> CsvWithOneColumnBiggerThanAnother => RandomCsv
        .Where(csv => csv is { Columns.Count: >= 2 })
        .Where(csv => csv.Values[0].Sum() != 0)
        .Select(csv => new SmallerAndBiggerCsv(
            Smaller: new Csv(
                Values: [csv.Values[0][..^1]],
                Columns: csv.Columns.ToArray()[..^1]),
            Bigger: csv));

    private static Gen<SmallerAndBiggerCsv> CsvWithOneValueSmallerThanOther => RandomCsv
        .Select(csv => csv with { Values = [csv.Values[0]] })
        .Where(csv => csv.Values[0][0] != 0)
        .Select(csv => new SmallerAndBiggerCsv(
            Bigger: csv,
            Smaller: csv with
            {
                Values =
                [
                    [
                        csv.Values[0][0] - 1,
                        ..csv.Values[0][1..],
                    ],
                ],
            }));


    private async Task<MemoryStream> CompressAsync(
        int[][] csv,
        IReadOnlyCollection<int> columns)
    {
        var stream = new MemoryStream();

        await _compressor
            .WriteCcaAsync(csv, columns, stream);

        return stream;
    }

    private static BigInteger GetMaxPossibleNumber(Csv csv) => csv
        .Columns
        .Aggregate(BigInteger.One, (current, column) => current * column);

    private static Gen<Csv> RandomCsvWithMaxValuesPerColumn => ColumnsGenerator
        .Zip(RowsGenerator, (columns, rows) =>
        {
            var values = Enumerable.Range(0, rows)
                .Select(_ => columns
                    .Select(x => x - 1)
                    .ToArray())
                .ToArray();

            return new Csv(values, columns);
        });

    private static Gen<Csv> RandomCsvWithZeros => ColumnsGenerator
        .Zip(RowsGenerator, (columns, rows) =>
        {
            var values = Enumerable.Range(0, rows)
                .Select(_ => Enumerable
                    .Repeat(0, columns.Count)
                    .ToArray())
                .ToArray();

            return new Csv(values, columns);
        });

    private static Gen<Csv> RandomCsv => ColumnsGenerator.Zip(
        RowsGenerator,
        (columns, row) => new Csv(GetCsv(row, columns), columns));

    private static BigInteger GetBigNumber(MemoryStream stream)
    {
        var bytes = stream.ToArray();

        var number = new BigInteger(bytes);

        return number;
    }

    private static Gen<int> ColumnGenerator => Arb
        .Default
        .PositiveInt()
        .Generator
        .Select(x => x.Get)
        .Select(x => x <= 2 ? 2 : x);

    private static Gen<ReadOnlyCollection<int>> ColumnsGenerator => ColumnGenerator
        .NonEmptyListOf()
        .Select(CollectionExtensions.AsReadOnly);

    private static Gen<int> RowsGenerator => Arb
        .Default
        .PositiveInt()
        .Generator
        .Select(x => x.Get);

    private static int[][] GetCsv(
        int rows,
        IReadOnlyCollection<int> realSizes) => new DefaultRandomCsvGeneratorFactory()
        .Create()
        .WithColumns(realSizes)
        .WithRowsCount(rows)
        .Generate()
        .Select(row => row.Select(int.Parse).ToArray())
        .ToArray();

    private sealed record SmallerAndBiggerCsv(
        Csv Smaller,
        Csv Bigger);

    private sealed record Csv(
        int[][] Values,
        IReadOnlyCollection<int> Columns);
}