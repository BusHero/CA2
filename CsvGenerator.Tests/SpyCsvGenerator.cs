namespace CsvGenerator.Tests;

using System.IO;

internal sealed class SpyCsvGenerator : IRandomCsvGenerator
{
    private string[][]? _csv;

    public int RowsCount { get; private set; }

    public List<ColumnDefinition> Columns { get; } = [];

    public IRandomCsvGenerator WithColumn(string[] column)
    {
        Columns.Add(new ValuesColumnDefinition(column));
        
        return this;
    }

    public IRandomCsvGenerator WithRowsCount(int rows)
    {
        RowsCount = rows;

        return this;
    }

    public void WithRandomCsv(string[][] csv)
        => _csv = csv;

    public IEnumerable<string[]> Generate()
        => _csv!;

    public void Generate(Stream stream) => throw new NotImplementedException();

    internal abstract record ColumnDefinition;

    internal sealed record ValuesColumnDefinition(
        string[] Values) : ColumnDefinition;
}