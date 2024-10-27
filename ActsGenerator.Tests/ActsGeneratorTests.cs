using FsCheck;
using FsCheck.Xunit;

using TestUtils;

namespace ActsGenerator.Tests;

public class ActsGeneratorTests
{
    public ActsGeneratorTests()
        => Arb.Register<Generators>();

    [Property]
    public Property ResultShouldContainExpectedRows(
        PositiveInt rows,
        ColumnSize column)
    {
        var generator = new ActsGenerator();

        var acts = generator
            .WithRows(rows.Get)
            .WithColumn(column.Get)
            .Generate()
            .ToArray();

        return (acts[0] == rows.Get.ToString())
            .ToProperty();
    }

    [Property]
    public Property ResultContainsTheSpecifiedNumberOfRows(
        PositiveInt rows,
        ColumnSize column)
    {
        var generator = new ActsGenerator();

        var acts = generator
            .WithRows(rows.Get)
            .WithColumn(column.Get)
            .Generate()
            .ToArray();

        return (acts.Length == rows.Get + 1)
            .ToProperty();
    }

    [Property]
    public Property TotalNumberOfDistinctValuesIsEqualToColumnSize(
        NonEmptyArray<ColumnSize> columns)
    {
        var generator = GetGenerator(columns);

        var acts = generator
            .Generate()
            .Skip(1)
            .Select(x => x.Split(' ').Select(int.Parse).ToArray())
            .ToArray()
            .Pivot()
            .Select(x => x.Distinct().Count())
            .ToArray();

        return acts
            .Select((x, i) => x == columns.Get[i].Get)
            .All(x => x)
            .ToProperty();
    }

    [Property]
    public Property EachRowContainTheSpecifiedNumberOfColumns(
        NonEmptyArray<ColumnSize> columns)
    {
        var generator = GetGenerator(columns);

        var acts = generator.Generate()
            .Skip(1)
            .Select(x => x.Split(' ').Length);

        return acts
            .All(x => x == columns.Get.Length)
            .ToProperty();
    }

    [Property]
    public Property AllValuesAreBetweenZeroAndColumnSize(
        NonEmptyArray<ColumnSize> columns)
    {
        var generator = GetGenerator(columns);

        var acts = generator.Generate()
            .Skip(1)
            .Select(x => x.Split(' ').Select(int.Parse).ToArray())
            .ToArray()
            .Pivot();

        return acts
            .Select((row, i) => row.All(item => 0 <= item && item <= columns.Get[i].Get))
            .All(x => x)
            .ToProperty();
    }

    private static ActsGenerator GetGenerator(NonEmptyArray<ColumnSize> columns)
    {
        var max = columns.Get.MaxBy(x => x.Get)!;
        var generator = columns.Get.Aggregate(
            new ActsGenerator()
                .WithRows(GetTrials(max.Get)),
            (gen, column) => gen.WithColumn(column.Get));
        return generator;
    }

    private static int GetTrials(int n)
        => (int)Math.Ceiling(Math.Log10(n) * n) + 1000;
}