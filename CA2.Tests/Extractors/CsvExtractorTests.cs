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
        using var stream = GetStreamWithCsv(columnSizes).Result;

        var pivot = GetCsv(stream).Pivot();

        return pivot.Zip(columnSizes.Get)
            .All(t => t.First.Distinct().Count() == t.Second.Get)
            .ToProperty();
    }

    [Property]
    public Property MinimumValueIsZero(
        NonEmptyArray<PositiveInt> columnSizes)
    {
        using var stream = GetStreamWithCsv(columnSizes).Result;

        var pivot = GetCsv(stream).Pivot();

        return pivot.All(row => row.Min() == 0)
            .ToProperty();
    }

    [Property]
    public Property MaximumValueIsColumnSizeMinus1(
        NonEmptyArray<PositiveInt> columnSizes)
    {
        using var stream = GetStreamWithCsv(columnSizes).Result;

        var pivot = GetCsv(stream).Pivot();

        return pivot
            .Zip(columnSizes.Get)
            .All(t => t.First.Max() == t.Second.Get - 1)
            .ToProperty();
    }

    [Property]
    public Property OptimizedCsvHasSameSizeAsOriginalCsv(
        NonEmptyArray<PositiveInt> columnSizes)
    {
        using var stream = GetStreamWithCsv(columnSizes).Result;

        var pivot = GetCsv(stream).Pivot();

        return (pivot.Length == columnSizes.Get.Length).ToProperty();
    }

    private static async Task<MemoryStream> GetStreamWithCsv(NonEmptyArray<PositiveInt> columnSizes)
    {
        var stream = new MemoryStream();
        var realColumn = columnSizes.Get
            .Select(x => x.Get)
            .ToArray();

        await new DefaultRandomCsvGeneratorFactory()
            .Create()
            .WithColumns(realColumn)
            .WithRowsCount(1_000)
            .GenerateAsync(stream);
        stream.Position = 0;
        
        return stream;
    }

    private static int[][] GetCsv(MemoryStream stream)
    {
        var reader = new StreamReader(stream);
        
        var csv = new CsvExtractor()
            .ExtractAsync(reader)
            .Result;
        
        return csv;
    }
}