namespace GeneratorLibrary;

public class RandomCsvGenerator
{
    private int _rows;
    private readonly List<string[]> _columns = [];

    private readonly Random _random = Random.Shared;

    public virtual string[][] Generate()
        => Enumerable.Range(0, _rows)
            .Select(_ => Enumerable
                .Range(0, _columns.Count)
                .Select(i => GetRandomValueFromRange(_columns[i]))
                .ToArray())
            .ToArray();

    private string GetRandomValueFromRange(string[] range)
        => range[_random.Next(range.Length)];

    public virtual RandomCsvGenerator WithRowsCount(int rows)
    {
        _rows = rows;

        return this;
    }

    public virtual RandomCsvGenerator WithColumn(string[] column)
    {
        _columns.Add(column);

        return this;
    }

    public virtual RandomCsvGenerator WithColumn(int numberOfValues)
    {
        var column = Enumerable
            .Range(0, numberOfValues)
            .Select(x => x.ToString()).ToArray();

        return WithColumn(column);
    }

    public virtual RandomCsvGenerator WithColumns(int[] columns)
        => columns
            .Aggregate(
                this,
                (gen, column) => gen.WithColumn(column));

    public virtual RandomCsvGenerator WithColumns(string[][] columns)
    {
        return columns
            .Aggregate(this, (gen, column) => gen.WithColumn(column));
    }
}