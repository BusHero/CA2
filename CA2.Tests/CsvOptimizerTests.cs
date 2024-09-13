namespace CA2.Tests;

using GeneratorLibrary;

public sealed class CsvOptimizerTests
{
    [Property]
    public Property OptimizedCsvContainsColumnSizeOfDistinctNumbers(
        NonEmptyArray<PositiveInt> columnSizes)
    {
        var csv = new RandomCsvGenerator()
            .WithColumns(columnSizes.Get.Select(x => x.Get).ToArray())
            .WithRowsCount(10_000)
            .Generate();

        var result = CsvOptimizer.Optimize(csv);
        var pivot = result.Pivot();

        return pivot.Zip(columnSizes.Get)
            .All(t => t.First.Distinct().Count() == t.Second.Get)
            .ToProperty();
    }

    [Property]
    public Property MinimumValueIsZero(
        NonEmptyArray<PositiveInt> columnSizes)
    {
        var csv = new RandomCsvGenerator()
            .WithColumns(columnSizes.Get.Select(x => x.Get).ToArray())
            .WithRowsCount(10_000)
            .Generate();

        var result = CsvOptimizer.Optimize(csv);
        var pivot = result.Pivot();

        return pivot.All(row => row.Min() == 0).ToProperty();
    }

    [Property]
    public Property MaximumValueIsColumnSizeMinus1(
        NonEmptyArray<PositiveInt> columnSizes)
    {
        var csv = new RandomCsvGenerator()
            .WithColumns(columnSizes.Get.Select(x => x.Get).ToArray())
            .WithRowsCount(10_000)
            .Generate();

        var result = CsvOptimizer.Optimize(csv);
        var pivot = result.Pivot();

        return pivot
            .Zip(columnSizes.Get)
            .All(t => t.First.Max() == t.Second.Get - 1)
            .ToProperty();
    }

    [Property]
    public Property OptimizedCsvHasSameSizeAsOriginalCsv(
        NonEmptyArray<PositiveInt> columnSizes)
    {
        var csv = new RandomCsvGenerator()
            .WithColumns(columnSizes.Get.Select(x => x.Get).ToArray())
            .WithRowsCount(10_000)
            .Generate();

        var result = CsvOptimizer.Optimize(csv);
        var pivot = result.Pivot();

        return (pivot.Length == columnSizes.Get.Length).ToProperty();
    }
}
