using System.Text.Json;

using CustomFluentAssertions;

using TestUtils;

namespace CsvGenerator.Tests;

public sealed class RandomCsvGeneratorTests
{
    [Property]
    public Property ReportContainsSpecifiedNumberOfRows(PositiveInt rows)
    {
        var generator = new RandomCsvGenerator();

        var csv = generator
            .WithRowsCount(rows.Get)
            .Generate()
            .ToArray();

        return (csv.Length == rows.Item).ToProperty();
    }

    [Property]
    public Property WithColumn_AllRowsHaveSpecifiedNumberOfItems(
        PositiveInt rows,
        NonEmptyArray<NonEmptyArray<string>> columns)
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

        var csv = generator.Generate().ToArray();
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

        var csv = generator.Generate().ToArray();
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

        var csv = generator.Generate().ToArray();
        var pivot = csv.Pivot();

        return pivot[0]
            .IsEvenlySpread(valuesForColumn.Item, 0.05).ToProperty();
    }

    [Property]
    public Property WithColumns_CsvReportHasSpecifiedNumberOfColumns(
        NonEmptyArray<PositiveInt> valuesForColumn)
    {
        var columns = valuesForColumn.Item.Select(x => x.Item).ToArray();
        var generator = new RandomCsvGenerator()
            .WithRowsCount(10000)
            .WithColumns(columns);

        var csv = generator.Generate();

        return csv
            .All(x => x.Length == valuesForColumn.Item.Length)
            .ToProperty();
    }

    [Property(Arbitrary = [typeof(Generators)])]
    public void ValuesAreWrittenIntoStream(
        PositiveInt seed,
        PositiveInt rows,
        NonEmptyArray<Column> columns)
    {
        using var stream = new MemoryStream();
        using var writer = new StreamWriter(stream);

        var realColumns = columns
            .Get
            .Select(x => x.Get)
            .ToArray();
        var csv1 = new RandomCsvGenerator(seed.Get)
            .WithRowsCount(rows.Get)
            .WithColumns(realColumns)
            .Generate();
        new RandomCsvGenerator(seed.Get)
            .WithRowsCount(rows.Get)
            .WithColumns(realColumns)
            .Generate(writer);
        stream.Position = 0;
        var csv2 = GetCsvFromStream(stream);

        csv1.Should().BeEquivalentTo(csv2);
    }

    private string[][] GetCsvFromStream(Stream stream)
    {
        var streamReader = new StreamReader(stream);

        var content = streamReader.ReadToEnd();
        return content.Split(Environment.NewLine).Select(x => x.Split(',')).ToArray();
    }

    [Property]
    public void SameSeedGeneratesSameValue(
        int seed,
        PositiveInt rows,
        NonEmptyArray<PositiveInt> columns)
    {
        var realColumns = columns
            .Get
            .Select(x => x.Get)
            .ToArray();

        var csv1 = new RandomCsvGenerator(seed)
            .WithRowsCount(rows.Get)
            .WithColumns(realColumns)
            .Generate();
        var csv2 = new RandomCsvGenerator(seed)
            .WithRowsCount(rows.Get)
            .WithColumns(realColumns)
            .Generate();

        csv1.Should().BeEquivalentTo(csv2);
    }

    [Property(Arbitrary = [typeof(Generators)])]
    public Property DifferentSeedsGenerateDifferentValues(
        PositiveInt seed1,
        PositiveInt seed2,
        NonEmptyArray<Column> columns)
    {
        var realColumns = columns
            .Get
            .Select(x => x.Get)
            .ToArray();

        var csv1 = new RandomCsvGenerator(seed1.Get)
            .WithRowsCount(100)
            .WithColumns(realColumns)
            .Generate();
        var csv2 = new RandomCsvGenerator(seed2.Get)
            .WithRowsCount(100)
            .WithColumns(realColumns)
            .Generate();

        var jsonCsv1 = JsonSerializer.Serialize(csv1);
        var jsonCsv2 = JsonSerializer.Serialize(csv2);

        return (jsonCsv1 != jsonCsv2).When(seed1.Get != seed2.Get);
    }

    [Property(Arbitrary = [typeof(Generators)])]
    public Property GeneratorsWithoutSeedsGenerateDifferentCsvs(
        NonEmptyArray<Column> columns)
    {
        var realColumns = columns
            .Get
            .Select(x => x.Get)
            .ToArray();

        var csv1 = new RandomCsvGenerator()
            .WithRowsCount(100)
            .WithColumns(realColumns)
            .Generate();
        var csv2 = new RandomCsvGenerator()
            .WithRowsCount(100)
            .WithColumns(realColumns)
            .Generate();

        var jsonCsv1 = JsonSerializer.Serialize(csv1);
        var jsonCsv2 = JsonSerializer.Serialize(csv2);

        return (jsonCsv1 != jsonCsv2).ToProperty();
    }
}

public static class Generators
{
    public static Arbitrary<Column> Generator()
        => Arb.Default.PositiveInt()
            .Generator
            .Select(x => 2 <= x.Get ? x.Get : 2)
            .Select(x => new Column(x))
            .ToArbitrary();
}

public record Column(int Get);