namespace CA2.Tests.CsvGenerationTests;

using GeneratorLibrary.CsvGenerators;

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

    public string[][] Generate()
        => _csv!;

    internal abstract record ColumnDefinition;

    internal sealed record ValuesColumnDefinition(
        string[] Values) : ColumnDefinition;
}