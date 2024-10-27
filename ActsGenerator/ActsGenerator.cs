namespace ActsGenerator;

public sealed class ActsGenerator
{
    private int _rows;
    private readonly Random _random = Random.Shared;
    private readonly List<int> _columns = [];

    public IEnumerable<string> Generate()
    {
        yield return _rows.ToString();

        for (var i = 0; i < _rows; i++)
        {
            var range = Enumerable
                .Range(0, _columns.Count)
                .Select(index => _random.Next(_columns[index]));
            
            yield return string.Join(' ', range);
        }
    }

    public ActsGenerator WithRows(int rows)
    {
        _rows = rows;
        
        return this;
    }

    public ActsGenerator WithColumn(int columnSize)
    {
        _columns.Add(columnSize);
        return this;
    }
}