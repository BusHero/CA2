namespace CsvGenerator;

public sealed class RandomCsvGenerator : IRandomCsvGenerator
{
    private int _rows;
    private readonly List<string[]> _columns = [];
    private readonly Random _random;

    public RandomCsvGenerator(int seed) => _random = new Random(seed);

    public RandomCsvGenerator() => _random = Random.Shared;

    public IEnumerable<string[]> Generate()
        => Enumerable.Range(0, _rows)
            .Select(_ => Enumerable
                .Range(0, _columns.Count)
                .Select(i => GetRandomValueFromRange(_columns[i]))
                .ToArray())
            .ToArray();

    private string GetRandomValueFromRange(string[] range)
        => range[_random.Next(range.Length)];

    public IRandomCsvGenerator WithRowsCount(int rows)
    {
        _rows = rows;

        return this;
    }

    public IRandomCsvGenerator WithColumn(string[] column)
    {
        _columns.Add(column);

        return this;
    }
}