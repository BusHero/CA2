namespace CA2.Tests.Compression;

using System.Collections.ObjectModel;
using System.Numerics;

using CA2.Compression;

using CsvGenerator;

using TestUtils;

using Utils;

public sealed class CompressorTests2
{
    private readonly Compressor2 _compressor = new();

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
        { [[0, 1]], [2, 3], 1 },
        { [[0, 2]], [2, 3], 2 },
        { [[1, 0]], [2, 3], 3 },
        { [[1, 1]], [2, 3], 4 },
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
        { [[1, 2, 1, 1, 1, 0, 0]], [2, 7, 3, 2, 2, 2, 2], 460 },
        { [[1, 0, 0, 1, 0, 0, 1, 4, 1]], [2, 2, 2, 2, 2, 2, 2, 7, 2], 1_031 },
        { [[1, 2, 1]], [2, 3, 2], 11 },
    };

    [Theory]
    [MemberData(nameof(Data))]
    public async Task GenerateGeneratesExpectedNumber(
        int[][] item,
        int[] sizes,
        int expectedResult)
    {
        using var stream = new MemoryStream();

        await _compressor.CompressAsync(
            item,
            sizes,
            stream);

        stream.Seek(0, SeekOrigin.Begin);

        var bytes = stream.ToArray();

        new BigInteger(bytes)
            .Should()
            .Be(expectedResult);
    }

    [Property]
    public Property ResultShouldBeBetweenZeroAndMaxValue() => Prop
        .ForAll(
            Gen.OneOf(
                    RandomCsv,
                    RandomCsvWithZeros,
                    RandomCsvWithMaxValuesPerColumn)
                .Select(x => x with { Values = [x.Values[0]] })
                .ToArbitrary(),
            csv =>
            {
                using var stream = new MemoryStream();

                _compressor
                    .CompressAsync(csv.Values, csv.Columns, stream)
                    .Wait();

                var result = GetBigNumber(stream);

                return 0 <= result && result < GetMaxPossibleNumber(csv);
            });

    [Property]
    public Property DifferentRowsGenerateDifferentValues() => Prop
        .ForAll(
            RandomCsv
                .Where(x => 2 < x.Values.Length)
                .Where(x => !x.Values[0].SequenceEqual(x.Values[1]))
                .Select(csv => (
                    csv with { Values = [csv.Values[0]] },
                    csv with { Values = [csv.Values[1]] }))
                .ToArbitrary(),
            t =>
            {
                using var stream1 = new MemoryStream();
                using var stream2 = new MemoryStream();

                _compressor
                    .CompressAsync(t.Item1.Values, t.Item1.Columns, stream1)
                    .Wait();
                _compressor
                    .CompressAsync(t.Item2.Values, t.Item2.Columns, stream2)
                    .Wait();

                return GetBigNumber(stream1) != GetBigNumber(stream2);
            });

    [Property]
    public Property SmallerCsvGeneratesSmallerNumberThanBiggerCsv() => Prop
        .ForAll(
            Gen.OneOf(
                CsvWithOneValueSmallerThanOther,
                CsvWithOneColumnBiggerThanAnother,
                CsvWithOneColumnBiggerThanOther).ToArbitrary(),
            t =>
            {
                using var smallerStream = new MemoryStream();
                using var biggerStream = new MemoryStream();

                _compressor
                    .CompressAsync(t.Smaller.Values, t.Smaller.Columns, smallerStream)
                    .Wait();
                _compressor
                    .CompressAsync(t.Bigger.Values, t.Bigger.Columns, biggerStream)
                    .Wait();

                var smaller = GetBigNumber(smallerStream);
                var bigger = GetBigNumber(biggerStream);

                return smaller < bigger;
            });

    [Property]
    public Property IsSorted() => Prop.ForAll(
        RandomCsv.Select(csv => csv with { Values = [csv.Values[0]] }).ToArbitrary(),
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
    
            using var sortedStream = new MemoryStream();
            using var originalStream = new MemoryStream();
    
            _compressor
                .CompressAsync(csv.Values, csv.Columns, sortedStream)
                .Wait();
            _compressor
                .CompressAsync(sortedCsv.Values, sortedCsv.Columns, originalStream)
                .Wait();
    
            var sortedResult = GetBigNumber(sortedStream);
            var originalResult = GetBigNumber(originalStream);
    
            return sortedResult == originalResult;
        });

    private static Gen<TwoCsvs> CsvWithOneColumnBiggerThanAnother => RandomCsv
        .Where(csv => 2 <= csv.Columns.Count)
        .Where(csv => csv.Values[0].Sum() != 0)
        .Select(csv => new TwoCsvs(
            Smaller: new Csv(
                Values: [csv.Values[0][..^1]],
                Columns: csv.Columns.ToArray()[..^1]),
            Bigger: csv));

    private static Gen<TwoCsvs> CsvWithOneColumnBiggerThanOther => RandomCsv
        .Select(csv => csv with { Values = [csv.Values[0]] })
        .Where(csv => 1 < csv.Columns.Count)
        .Where(csv => csv.Values[0][0] != 0)
        .Select(csv =>
        {
            var columns = csv.Columns.ToList();
            columns[^1] += 1;

            return
                new TwoCsvs(
                    Smaller: csv,
                    Bigger: csv with { Columns = columns });
        });


    private record TwoCsvs(
        Csv Smaller,
        Csv Bigger);

    private static Gen<TwoCsvs> CsvWithOneValueSmallerThanOther => RandomCsv
        .Select(csv => csv with { Values = [csv.Values[0]] })
        .Where(csv => csv.Values[0][0] != 0)
        .Select(csv => new TwoCsvs(
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


    private static BigInteger GetMaxPossibleNumber(Csv csv)
    {
        var maxNumber = csv
            .Columns
            .Aggregate(BigInteger.One, (current, column) => current * column);
        return maxNumber;
    }

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

    private static Gen<Csv> RandomCsv => ColumnsGenerator
        .Zip(RowsGenerator, (columns, row) => new Csv(GetCsv(row, columns), columns));

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

#pragma warning disable IDE0051
    private static Gen<int> RowsGenerator => Arb
#pragma warning restore IDE0051
        .Default
        .PositiveInt()
        .Generator
        .Select(x => x.Get);

    private static int[][] GetCsv(
        int rows,
        IReadOnlyCollection<int> realSizes)
        => new DefaultRandomCsvGeneratorFactory()
            .Create()
            .WithColumns(realSizes)
            .WithRowsCount(rows)
            .Generate()
            .Select(row => row.Select(int.Parse).ToArray())
            .ToArray();

    private sealed record Csv(
        int[][] Values,
        IReadOnlyCollection<int> Columns);
}

public sealed class CompressorTests
{
    private readonly Compressor _compressor = new();

    public static TheoryData<Combination, int> Data => new()
    {
        { new Combination { Item = [0, 0], Sizes = [2, 2] }, 0 },
        { new Combination { Item = [0, 1], Sizes = [2, 2] }, 1 },
        { new Combination { Item = [1, 0], Sizes = [2, 2] }, 2 },
        { new Combination { Item = [1, 1], Sizes = [2, 2] }, 3 },

        { new Combination { Item = [0, 0], Sizes = [3, 2] }, 0 },
        { new Combination { Item = [0, 1], Sizes = [3, 2] }, 1 },
        { new Combination { Item = [1, 0], Sizes = [3, 2] }, 2 },
        { new Combination { Item = [1, 1], Sizes = [3, 2] }, 3 },
        { new Combination { Item = [2, 0], Sizes = [3, 2] }, 4 },
        { new Combination { Item = [2, 1], Sizes = [3, 2] }, 5 },

        { new Combination { Item = [0, 0], Sizes = [2, 3] }, 0 },
        { new Combination { Item = [0, 1], Sizes = [2, 3] }, 1 },
        { new Combination { Item = [0, 2], Sizes = [2, 3] }, 2 },
        { new Combination { Item = [1, 0], Sizes = [2, 3] }, 3 },
        { new Combination { Item = [1, 1], Sizes = [2, 3] }, 4 },
        { new Combination { Item = [1, 2], Sizes = [2, 3] }, 5 },

        { new Combination { Item = [0, 0], Sizes = [3, 3] }, 0 },
        { new Combination { Item = [0, 1], Sizes = [3, 3] }, 1 },
        { new Combination { Item = [0, 2], Sizes = [3, 3] }, 2 },
        { new Combination { Item = [1, 0], Sizes = [3, 3] }, 3 },
        { new Combination { Item = [1, 1], Sizes = [3, 3] }, 4 },
        { new Combination { Item = [1, 2], Sizes = [3, 3] }, 5 },
        { new Combination { Item = [2, 0], Sizes = [3, 3] }, 6 },
        { new Combination { Item = [2, 1], Sizes = [3, 3] }, 7 },
        { new Combination { Item = [2, 2], Sizes = [3, 3] }, 8 },

        { new Combination { Item = [0, 0, 0], Sizes = [2, 2, 2] }, 0 },
        { new Combination { Item = [0, 0, 1], Sizes = [2, 2, 2] }, 1 },
        { new Combination { Item = [0, 1, 0], Sizes = [2, 2, 2] }, 2 },
        { new Combination { Item = [0, 1, 1], Sizes = [2, 2, 2] }, 3 },
        { new Combination { Item = [1, 0, 0], Sizes = [2, 2, 2] }, 4 },
        { new Combination { Item = [1, 0, 1], Sizes = [2, 2, 2] }, 5 },
        { new Combination { Item = [1, 1, 0], Sizes = [2, 2, 2] }, 6 },
        { new Combination { Item = [1, 1, 1], Sizes = [2, 2, 2] }, 7 },
        { new Combination { Item = [1, 1, 0, 0], Sizes = [2, 2, 2, 2] }, 12 },
        { new Combination { Item = [1, 2, 1, 1, 1, 0, 0], Sizes = [2, 7, 3, 2, 2, 2, 2] }, 460 },
        { new Combination { Item = [1, 0, 0, 1, 0, 0, 1, 4, 1], Sizes = [2, 2, 2, 2, 2, 2, 2, 7, 2] }, 1_031 },
        { new Combination { Item = [1, 2, 1], Sizes = [2, 3, 2] }, 11 },
    };

    [Theory, AutoData]
    public void ValuesIsNullThrows(int[] sizes)
    {
        var func = () => Compressor.Compress(
            null!,
            sizes);

        func.Should()
            .Throw<ArgumentNullException>();
    }

    [Theory, AutoData]
    public void SizesIsNullThrows(int[] values)
    {
        var func = () => Compressor.Compress(
            values,
            null!);

        func.Should()
            .Throw<ArgumentNullException>();
    }

    [Fact]
    public void ValuesLengthIsDifferentFromSizesLength_ThrowInvalidOperationException()
    {
        int[] values = [1, 1, 1];
        int[] sizes = [4, 4];

        var func = () => Compressor.Compress(
            values,
            sizes);

        func.Should()
            .Throw<InvalidOperationException>();
    }

    [Fact]
    public void ValuesContainNegativeNumbers_ThrowInvalidOperationException()
    {
        int[] values = [-1, 1, 1];
        int[] sizes = [4, 4, 4];

        var func = () => Compressor.Compress(
            values,
            sizes);

        func.Should()
            .Throw<InvalidOperationException>();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    public void SizesContainElementsSmallerOrEqualTo1_ThrowInvalidOperationException(
        int size)
    {
        var prop = () => Compressor.Compress(
            [0],
            [size]);

        prop.Should()
            .Throw<InvalidOperationException>();
    }

    [Theory]
    [InlineData(10, 10)]
    [InlineData(11, 10)]
    public void ValuesContainElementsBiggerThanSizes_ThrowInvalidOperationException(
        int value,
        int size)
    {
        var property = () => Compressor.Compress(
            [value],
            [size]);

        property.Should()
            .Throw<InvalidOperationException>();
    }

    [Theory]
    [MemberData(nameof(Data))]
    public void GenerateGeneratesExpectedNumber(
        Combination combination,
        int expectedResult)
    {
        var number = Compressor.Compress(
            combination.Item,
            combination.Sizes);

        number.Should()
            .Be(expectedResult);
    }

    [Property]
    public Property ResultIsBiggerOrEqualToZero()
    {
        var arb = Gen
            .Sized(size => Gen.Elements(0, 1)
                .ArrayOf(size))
            .ToArbitrary();

        return Prop
            .ForAll(
                arb,
                numbers =>
                {
                    var sizes = GetSizes(numbers.Length);

                    var number = Compressor.Compress(
                        numbers,
                        sizes);

                    var property = 0 <= number;

                    return property
                        .Label($"{number} should be bigger than 0");
                });
    }

    [Property]
    public Property ResultIsSmallerOrEqualToTheCount()
    {
        var arb = Gen
            .Sized(size => Gen
                .Elements(0, 1)
                .ArrayOf(size))
            .ToArbitrary();

        return Prop
            .ForAll(
                arb,
                numbers =>
                {
                    var sizes = GetSizes(numbers.Length);

                    var number = Compressor.Compress(numbers, sizes);

                    var powerOfTwo = BigInteger.Pow(2, numbers.Length);

                    var property = number < powerOfTwo;

                    return property
                        .Label($"{number} < {powerOfTwo}");
                });
    }

    [Property]
    public Property ArraysOfZerosHaveZero()
    {
        var arb = Gen
            .Sized(size => Gen
                .Elements(0)
                .ArrayOf(size))
            .ToArbitrary();

        return Prop.ForAll(
            arb,
            numbers =>
            {
                var sizes = GetSizes(numbers.Length);

                var number = Compressor.Compress(
                    numbers,
                    sizes);

                var property = number == 0;

                return property
                    .Label($"{number} == 0");
            });
    }

    [Property]
    public Property ArraysOfOnesHaveMaxPower()
    {
        var arb = Gen
            .Sized(size => Gen
                .Elements(1)
                .ArrayOf(size))
            .ToArbitrary();

        return Prop.ForAll(
            arb,
            numbers =>
            {
                var sizes = GetSizes(numbers.Length);

                var number = Compressor.Compress(numbers, sizes);

                var biggestPossibleNumber = BigInteger.Pow(2, numbers.Length) - 1;
                var property = number == biggestPossibleNumber;

                return property.Label($"{number} == {biggestPossibleNumber}");
            });
    }

    [Property(Arbitrary = [typeof(CombinationsGenerator)])]
    public Property NumberIsSmallerThanMaximumPossibleSize(
        Combination combination)
    {
        var result = Compressor.Compress(
            combination.Item,
            combination.Sizes);

        var maximumPossibleNumber = combination.Sizes.CalculateMaximumNumber();

        var property = result <= maximumPossibleNumber;

        return property
            .Label($"{result} is smaller than {maximumPossibleNumber}");
    }

    [Property]
    public void StreamContainsExpectedNumberOfBytes(
        NonEmptyArray<PositiveInt> sizes,
        PositiveInt rows,
        byte interactionStrength)
    {
        using var ccaStream = new MemoryStream();
        using var metaStream = new MemoryStream();

        var realSizes = GetRealColumns(sizes);

        _compressor.CompressAsync(
            GetCsv(rows, realSizes),
            realSizes,
            interactionStrength,
            ccaStream,
            metaStream).Wait();
        ccaStream.Position = 0;

        ccaStream
            .Should()
            .HaveLength(realSizes.CalculateMaximumNumber().GetByteCount() * rows.Get);
    }

    [Property]
    public void AllRowsShouldHaveExpectedNumberOfItems(
        NonEmptyArray<PositiveInt> sizes,
        PositiveInt rows1)
    {
        using var ccaStream = new MemoryStream();

        _compressor.CompressAsync(
            GetCsv(rows1, GetRealColumns(sizes)),
            GetRealColumns(sizes),
            ccaStream).Wait();
        ccaStream.Position = 0;

        var rows = _compressor.Decompress(GetRealColumns(sizes), ccaStream);

        rows.Should().AllSatisfy(x => x.Should().HaveSameCount(sizes.Get));
    }

    [Property]
    public void StreamContainsExpectedNumberOfRows(
        NonEmptyArray<PositiveInt> sizes,
        PositiveInt rows1)
    {
        using var ccaStream = new MemoryStream();

        _compressor.CompressAsync(
            GetCsv(rows1, GetRealColumns(sizes)),
            GetRealColumns(sizes),
            ccaStream).Wait();

        ccaStream.Position = 0;

        var rows = _compressor.Decompress(GetRealColumns(sizes), ccaStream);

        rows.Should().HaveCount(rows1.Get);
    }


    private static int[] GetRealColumns(NonEmptyArray<PositiveInt> values)
        => values
            .Get
            .Select(x => x.Get)
            .Select(x => 2 < x ? x : 2)
            .ToArray();

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

    private static int[] GetSizes(int numbersLength)
        => Enumerable
            .Repeat(2, numbersLength)
            .ToArray();
}