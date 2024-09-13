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
        var pivot = result.Csv.Pivot();

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
        var pivot = result.Csv.Pivot();

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
        var pivot = result.Csv.Pivot();

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
        var pivot = result.Csv.Pivot();

        return (pivot.Length == columnSizes.Get.Length).ToProperty();
    }

    [Property]
    public Property Map(NonEmptyArray<NonEmptyArray<NonEmptyString>> columns)
    {
        var csvGenerator = new RandomCsvGenerator()
            .WithRowsCount(10000);
        foreach (var column in columns.Get)
        {
            var realColumn = column.Item.Select(x => x.Item).ToArray();

            csvGenerator.WithColumn(realColumn);
        }
        var csv = csvGenerator.Generate();

        var result = CsvOptimizer.Optimize(csv);

        var valuesMap = result.ValuesMap;
        var foo = valuesMap
            .Select(x => x.Keys)
            .Zip(columns.Get.Select(x => x.Get.Select(y => y.Get).Distinct().ToArray()).Distinct())
    }
}
