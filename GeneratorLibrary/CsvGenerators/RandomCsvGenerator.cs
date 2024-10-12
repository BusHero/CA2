namespace GeneratorLibrary.CsvGenerators;

public interface IRandomCsvGenerator
{
    string[][] Generate();
    
    IRandomCsvGenerator WithRowsCount(int rows);
    
    IRandomCsvGenerator WithColumn(string[] column);
    
    IRandomCsvGenerator WithColumn(int numberOfValues);
    
    IRandomCsvGenerator WithColumns(int[] columns);
    
    IRandomCsvGenerator WithColumns(string[][] columns);
}

public sealed class RandomCsvGenerator : IRandomCsvGenerator
{
    private int _rows;
    private readonly List<string[]> _columns = [];

    private readonly Random _random = Random.Shared;

    public string[][] Generate()
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

    public IRandomCsvGenerator WithColumn(int numberOfValues)
    {
        var column = Enumerable
            .Range(0, numberOfValues)
            .Select(x => x.ToString())
            .ToArray();

        return WithColumn(column);
    }

    public IRandomCsvGenerator WithColumns(int[] columns)
        => columns
            .Aggregate(
                this as IRandomCsvGenerator,
                (gen, column) => gen.WithColumn(column));

    public IRandomCsvGenerator WithColumns(string[][] columns) 
        => columns
            .Aggregate(
                this as IRandomCsvGenerator, 
                (gen, column) => gen.WithColumn(column));
}