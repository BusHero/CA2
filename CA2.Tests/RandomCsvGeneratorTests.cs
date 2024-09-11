namespace CA2.Tests;

public sealed class RandomCsvGeneratorTests
{
    [Property]
    public Property ReportContainsSpecifiedNumberOfRows(PositiveInt rows)
    {
        var generator = new RandomCsvGenerator();

        var csv = generator
            .WithRowsCount(rows.Get)
            .Generate();

        return (csv.Length == rows.Item).ToProperty();
    }

    [Property]
    public Property WithColumn(PositiveInt rows, PositiveInt columns)
    {
        var generator = Enumerable.Range(0, columns.Get)
            .Aggregate(
                new RandomCsvGenerator().WithRowsCount(rows.Item),
                (gen, _) => gen.WithColumn());

        var csv = generator.Generate();

        return csv
            .All(row => row.Length == columns.Item)
            .ToProperty();
    }

    [Property]
    public Property WithColumn2(PositiveInt rows, NonEmptyArray<NonEmptyArray<string>> columns)
    {
        var generator = columns
            .Item
            .Select(x => x.Item)
            .Aggregate(
                new RandomCsvGenerator().WithRowsCount(rows.Get),
                (gen, column) => gen.WithColumn(column));

        var csv = generator.Generate();

        return csv
            .All(row => row.Length == columns.Item.Length)
            .ToProperty();
    }

    [Property]
    public Property ColumnsContainValuesFromTheSetupPhase(
        PositiveInt rows,
        NonEmptyArray<NonEmptyArray<string>> columns)
    {
        var generator = columns
            .Item
            .Select(x => x.Item)
            .Aggregate(
                new RandomCsvGenerator().WithRowsCount(rows.Item),
                (gen, column) => gen.WithColumn(column));

        var csv = generator.Generate();

        return csv
            .All(row => row
                .Select((cell, i) => columns.Item[i].Item.Contains(cell))
                .All(x => x))
            .ToProperty();
    }

    [Property]
    public Property ColumnsShouldHaveValuesFromAnyFoo(
        NonEmptyArray<NonEmptyArray<NonEmptyString>> columns)
    {
        var generator = columns
            .Item
            .Select(x => x.Item.Select(y => y.Get).ToArray())
            .Aggregate(
                new RandomCsvGenerator().WithRowsCount(1000),
                (gen, column) => gen.WithColumn(column));

        var csv = generator.Generate();
        var pivot = csv.Pivot();

        return pivot
            .Select((row, i) => row.Distinct().Count() == columns.Item[i].Item.Distinct().Count())
            .All(x => x)
            .ToProperty();
    }

    [Property]
    public Property ValuesInColumnsAreUniformlyDistributed(
        NonEmptyArray<NonEmptyArray<NonEmptyString>> columns)
    {
        var generator = columns
            .Item
            .Select(x => x.Item.Select(y => y.Get).ToArray())
            .Aggregate(
                new RandomCsvGenerator().WithRowsCount(10000),
                (gen, column) => gen.WithColumn(column));

        var csv = generator.Generate();
        var pivot = csv.Pivot();

        return pivot
            .Select((row, i) => row
                .IsEvenlySpread(columns.Item[i].Item.Distinct().Count(), 0.5))
            .All(x => x).ToProperty();
    }

    [Property]
    public Property Column5(PositiveInt valuesForColumn)
    {
        var generator = new RandomCsvGenerator()
            .WithRowsCount(10000)
            .WithColumn(valuesForColumn.Item);

        var csv = generator.Generate();
        var pivot = csv.Pivot();

        return (pivot[0].Distinct().Count() == valuesForColumn.Item).ToProperty();
    }
}
