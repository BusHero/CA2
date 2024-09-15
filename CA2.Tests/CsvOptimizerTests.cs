namespace CA2.Tests;

using GeneratorLibrary;

public sealed class CsvOptimizerTests
{
    [Property]
    public Property OptimizedCsvContainsColumnSizeOfDistinctNumbers(
        NonEmptyArray<PositiveInt> columnSizes)
    {
        var realColumn = columnSizes.Get
            .Select(x => x.Get)
            .ToArray();

        var csv = new RandomCsvGenerator()
            .WithColumns(realColumn)
            .WithRowsCount(1_000)
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
        var realColumn = columnSizes.Get
            .Select(x => x.Get)
            .ToArray();

        var csv = new RandomCsvGenerator()
            .WithColumns(realColumn)
            .WithRowsCount(1_000)
            .Generate();

        var result = CsvOptimizer.Optimize(csv);
        var pivot = result.Csv.Pivot();

        return pivot.All(row => row.Min() == 0)
            .ToProperty();
    }

    [Property]
    public Property MaximumValueIsColumnSizeMinus1(
        NonEmptyArray<PositiveInt> columnSizes)
    {
        var realColumn = columnSizes.Get
            .Select(x => x.Get)
            .ToArray();
        
        var csv = new RandomCsvGenerator()
            .WithColumns(realColumn)
            .WithRowsCount(1_000)
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
        var realColumn = columnSizes.Get
            .Select(x => x.Get)
            .ToArray();
        var csv = new RandomCsvGenerator()
            .WithColumns(realColumn)
            .WithRowsCount(1_000)
            .Generate();

        var result = CsvOptimizer.Optimize(csv);
        var pivot = result.Csv.Pivot();

        return (pivot.Length == columnSizes.Get.Length).ToProperty();
    }

    [Property]
    public Property ValuesMap(NonEmptyArray<NonEmptyArray<Guid>> columns)
    {
        var realColumns = columns.Get
            .Select(x => x.Get
                .Select(y => y.ToString())
                .ToArray())
            .ToArray();

        var csv = realColumns
            .Aggregate(
                new RandomCsvGenerator().WithRowsCount(1_000),
                (gen, column) => gen.WithColumn(column))
            .Generate();

        var report = CsvOptimizer.Optimize(csv);

        return report.ValuesMap
            .Zip(realColumns)
            .All(t => t.First.Count == t.Second.Length)
            .ToProperty();
    }

    [Property]
    public Property ValuesMap_MinimumValueInMapIsZero(NonEmptyArray<NonEmptyArray<Guid>> columns)
    {
        var realColumns = columns.Get
            .Select(x => x.Get
                .Select(y => y.ToString())
                .ToArray())
            .ToArray();

        var csv = realColumns
            .Aggregate(
                new RandomCsvGenerator().WithRowsCount(1_000),
                (gen, column) => gen.WithColumn(column))
            .Generate();

        var report = CsvOptimizer.Optimize(csv);

        return report.ValuesMap
            .All(x => x.Values.Min() == 0)
            .ToProperty();
    }

    [Property]
    public Property ValuesMap_MaximumNumberInMapIsColumnSizeMinus1(NonEmptyArray<NonEmptyArray<Guid>> columns)
    {
        var realColumns = columns.Get
            .Select(x => x.Get
                .Select(y => y.ToString())
                .ToArray())
            .ToArray();

        var csv = realColumns
            .Aggregate(
                new RandomCsvGenerator().WithRowsCount(1_000),
                (gen, column) => gen.WithColumn(column))
            .Generate();

        var report = CsvOptimizer.Optimize(csv);

        return report.ValuesMap
            .Zip(realColumns)
            .All(t => t.First.Values.Max() == t.Second.Length - 1)
            .ToProperty();
    }

    [Property]
    public Property ValuesMap_KeysAreSameAsColumns(NonEmptyArray<NonEmptyArray<Guid>> columns)
    {
        var realColumns = columns.Get
            .Select(x => x.Get
                .Select(y => y.ToString())
                .ToArray())
            .ToArray();

        var csv = realColumns
            .Aggregate(
                new RandomCsvGenerator().WithRowsCount(1_000),
                (gen, column) => gen.WithColumn(column))
            .Generate();

        var report = CsvOptimizer.Optimize(csv);

        return report.ValuesMap
            .Zip(realColumns)
            .All(t =>
            {
                var first = t.First
                    .Keys
                    .Order()
                    .ToArray();

                var second = t.Second
                    .Order()
                    .ToArray();

                return first.SequenceEqual(second);
            })
            .ToProperty();
    }
}