namespace CA2.Tests.CsvGenerationTests;

using System.Diagnostics.CodeAnalysis;

using GeneratorLibrary;

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

    public IRandomCsvGenerator WithColumn(int numberOfValues)
    {
        Columns.Add(new CountColumnDefinition(numberOfValues));

        return this;
    }

    public IRandomCsvGenerator WithColumns(int[] columns)
    {
        var columnDefinitions = columns.Select(x => new CountColumnDefinition(x));
        Columns.AddRange(columnDefinitions);

        return this;
    }

    public IRandomCsvGenerator WithColumns(string[][] columns)
    {
        var columnDefinitions = columns
            .Select(c => new ValuesColumnDefinition(c));
        Columns.AddRange(columnDefinitions);

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

    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    [SuppressMessage("ReSharper", "NotAccessedPositionalProperty.Global")]
    internal sealed record CountColumnDefinition(
        int Count) : ColumnDefinition;
}