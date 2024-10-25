namespace CsvGenerator.Console.Tests;

using System.Collections.Generic;

public class MockCsvGenerator(IEnumerable<string[]> csv) : ICsvGenerator
{
    public List<string[]> Columns { get; private set; } = [];

    public int? RowsCount { get; private set; }

    public IEnumerable<string[]> Generate() => csv;

    public ICsvGenerator WithColumn(string[] column)
    {
        Columns.Add(column);

        return this;
    }

    public ICsvGenerator WithRowsCount(int rows)
    {
        RowsCount = rows;

        return this;
    }
}
