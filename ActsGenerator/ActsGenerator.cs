namespace ActsGenerator;

public sealed class ActsGenerator
{
    private int _rows;
    private readonly Random _random;
    private readonly List<int> _columns = [];
    private bool _emptyLineAtEnd;
    private bool _addWhiteSpace;

    public ActsGenerator(int seed)
    {
        _random = new Random(seed);
    }

    public ActsGenerator()
    {
        _random = Random.Shared;
    }

    public IEnumerable<string> Generate()
    {
        yield return _addWhiteSpace ? _rows.ToString() + ' ' : _rows.ToString();

        for (var i = 0; i < _rows; i++)
        {
            var range = Enumerable
                .Range(0, _columns.Count)
                .Select(index => _random.Next(_columns[index]));

            var value = string.Join(' ', range);

            if (_addWhiteSpace)
            {
                value += ' ';
            }

            yield return value;
        }

        if (_emptyLineAtEnd)
        {
            yield return _addWhiteSpace ? " " : string.Empty;
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

    public ActsGenerator AddEmptyLineAtEnd()
    {
        _emptyLineAtEnd = true;

        return this;
    }

    public ActsGenerator AddWhiteSpace()
    {
        _addWhiteSpace = true;
        return this;
    }
}