namespace CA2.Tests;

using Random = System.Random;

internal sealed class RandomCsvGenerator
{
    private int _rows = 0;
    private readonly List<string[]> _columns = [];

    private readonly Random _random = Random.Shared;

    internal string[][] Generate()
        => Enumerable.Range(0, _rows)
            .Select(_ => Enumerable
                .Range(0, _columns.Count)
                .Select(i => GetRandomValueFromRange(_columns[i]))
                .ToArray())
            .ToArray();

    private string GetRandomValueFromRange(string[] range) 
        => range[_random.Next(range.Length)];

    internal RandomCsvGenerator WithRowsCount(int rows)
    {
        _rows = rows;

        return this;
    }

    public RandomCsvGenerator WithColumn()
    {
        _columns.Add(["foo"]);

        return this;
    }

    public RandomCsvGenerator WithColumn(string[] column)
    {
        _columns.Add(column);

        return this;
    }

    public RandomCsvGenerator WithColumn(int numberOfValues)
    {
        var column = Enumerable
            .Range(0, numberOfValues)
            .Select(x => x.ToString()).ToArray();

        return WithColumn(column);
    }
}