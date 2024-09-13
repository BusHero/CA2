namespace CA2.Tests;

using GeneratorLibrary;

public sealed class CsvOptimizerTests
{
    [Property]
    public Property OptimizedCsvContainsColumnSizeOfDistinctNumbers(PositiveInt columnSize)
    {
        var csv = new RandomCsvGenerator()
            .WithColumn(columnSize.Get)
            .WithRowsCount(10_000)
            .Generate();

        var result = CsvOptimizer.Optimize(csv);
        var pivot = result.Pivot();

        return (pivot[0].Distinct().Count() == columnSize.Get).ToProperty();
    }

    [Property]
    public Property MinimumValueIsZero(PositiveInt columnSize)
    {
        var csv = new RandomCsvGenerator()
            .WithColumn(columnSize.Get)
            .WithRowsCount(10_000)
            .Generate();

        var result = CsvOptimizer.Optimize(csv);
        var pivot = result.Pivot();

        return (pivot[0].Min() == 0).ToProperty();
    }

    [Property]
    public Property MaximumValueIsColumnSizeMinus1(PositiveInt columnSize)
    {
        var csv = new RandomCsvGenerator()
            .WithColumn(columnSize.Get)
            .WithRowsCount(10_000)
            .Generate();

        var result = CsvOptimizer.Optimize(csv);
        var pivot = result.Pivot();

        return (pivot[0].Max() == columnSize.Get - 1).ToProperty();
    }
}
