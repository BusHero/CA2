namespace CA2.Tests;

using System;

public sealed class CsvOptimizerTests
{

    [Property]
    public Property GeneratorIsNeverEmpty()
    {
        var arb = Generator();

        return Prop.ForAll(arb, rows =>
        {
            return 0 < rows.Length;
        });
    }

    [Property]
    public Property ColumnsAreNeverEmpty()
    {
        var arb = Generator();

        return Prop.ForAll(arb, rows =>
        {
            return rows.All(row => 0 < row.Length);
        });
    }

    [Property]
    public Property AllRowsHaveSameLength()
    {
        var arb = Generator();

        return Prop.ForAll(arb, rows =>
        {
            var uniqueValues = rows
                .Select(row => row.Length)
                .Distinct()
                .Count();

            return uniqueValues == 1;
        });
    }

    private static Arbitrary<string[][]> Generator()
    {
        var arb = Arb
            .Default
            .NonEmptyArray<string>()
            .Generator
            .Select(item => new string[][] { item.Item })
            .ToArbitrary();

        return arb;
    }
}
