namespace CA2.Tests.CsvGenerationTests;

using System.Diagnostics.CodeAnalysis;

using GeneratorLibrary;

internal sealed class SpyCsvGenerator : RandomCsvGenerator
{
    private string[][]? _csv;

    public int RowsCount { get; private set; }

    public List<ColumnDefinition> Columns { get; } = [];

    public override RandomCsvGenerator WithColumn(string[] column)
    {
        Columns.Add(new ValuesColumnDefinition(column));
        return this;
    }

    public override RandomCsvGenerator WithColumn(int numberOfValues)
    {
        Columns.Add(new CountColumnDefinition(numberOfValues));

        return this;
    }

    public override RandomCsvGenerator WithColumns(int[] columns)
    {
        var columnDefinitions = columns.Select(x => new CountColumnDefinition(x));
        Columns.AddRange(columnDefinitions);

        return this;
    }

    public override RandomCsvGenerator WithColumns(string[][] columns)
    {
        var columnDefinitions = columns
            .Select(c => new ValuesColumnDefinition(c));
        Columns.AddRange(columnDefinitions);
        
        return this;
    }

    public override RandomCsvGenerator WithRowsCount(int rows)
    {
        RowsCount = rows;

        return this;
    }

    public void WithRandomCsv(string[][] csv)
        => _csv = csv;

    public override string[][] Generate()
        => _csv!;

    internal abstract record ColumnDefinition;

    internal sealed record ValuesColumnDefinition(
        string[] Values) : ColumnDefinition;

    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    [SuppressMessage("ReSharper", "NotAccessedPositionalProperty.Global")]
    internal sealed record CountColumnDefinition(
        int Count) : ColumnDefinition;
}