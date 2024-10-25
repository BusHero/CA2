namespace CA2.Tests.Extractors;

using CA2.Extractors;

using CsvGenerator;

using TestUtils;

public sealed class CsvExtractorTests
{
    [Property]
    public Property OptimizedCsvContainsColumnSizeOfDistinctNumbers(
        NonEmptyArray<PositiveInt> columnSizes)
    {
        using var stream = new MemoryStream();
        using var writer = new StreamWriter(stream);

        WriteCsv(columnSizes, writer);

        stream.Position = 0;
        using var reader = new StreamReader(stream);
        var csv = new CsvExtractor()
            .ExtractAsync(reader)
            .Result;
        var pivot = csv.Pivot();

        return pivot.Zip(columnSizes.Get)
            .All(t => t.First.Distinct().Count() == t.Second.Get)
            .ToProperty();
    }

    private static void WriteCsv(NonEmptyArray<PositiveInt> columnSizes, StreamWriter writer)
    {
        var realColumn = columnSizes.Get
            .Select(x => x.Get)
            .ToArray();

        new DefaultRandomCsvGeneratorFactory()
            .Create()
            .WithColumns(realColumn)
            .WithRowsCount(1_000)
            .GenerateAsync(writer)
            .Wait();
    }

    [Property]
    public Property MinimumValueIsZero(
        NonEmptyArray<PositiveInt> columnSizes)
    {
        using var stream = new MemoryStream();
        using var writer = new StreamWriter(stream);

        WriteCsv(columnSizes, writer);

        stream.Position = 0;
        using var reader = new StreamReader(stream);
        var csv = new CsvExtractor()
            .ExtractAsync(reader)
            .Result;
        var pivot = csv.Pivot();

        return pivot.All(row => row.Min() == 0)
            .ToProperty();
    }

    [Property]
    public Property MaximumValueIsColumnSizeMinus1(
        NonEmptyArray<PositiveInt> columnSizes)
    {
        using var stream = new MemoryStream();
        using var writer = new StreamWriter(stream);

        WriteCsv(columnSizes, writer);

        stream.Position = 0;
        using var reader = new StreamReader(stream);
        var csv = new CsvExtractor()
            .ExtractAsync(reader)
            .Result;
        var pivot = csv.Pivot();

        return pivot
            .Zip(columnSizes.Get)
            .All(t => t.First.Max() == t.Second.Get - 1)
            .ToProperty();
    }

    [Property]
    public Property OptimizedCsvHasSameSizeAsOriginalCsv(
        NonEmptyArray<PositiveInt> columnSizes)
    {
        using var stream = new MemoryStream();
        using var writer = new StreamWriter(stream);

        WriteCsv(columnSizes, writer);

        stream.Position = 0;
        using var reader = new StreamReader(stream);
        var csv = new CsvExtractor()
            .ExtractAsync(reader)
            .Result;
        var pivot = csv.Pivot();

        return (pivot.Length == columnSizes.Get.Length).ToProperty();
    }
}
