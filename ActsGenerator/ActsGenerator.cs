namespace ActsGenerator;

public sealed class ActsGenerator
{
    private const int ProbabilityToGenerateAMask = 20;
    
    private int _rows;
    private readonly Random _random;
    private readonly List<int> _columns = [];
    private bool _emptyLineAtEnd;
    private bool _addWhiteSpace;
    private bool _includeMaskForIrrelevantValues;

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
                .Select(index => GenerateRandomValue(_columns[index]));

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

    public async Task GenerateAsync(TextWriter writer)
    {
        var content = string.Join(Environment.NewLine, Generate());
        await writer.WriteAsync(content);
        await writer.FlushAsync();
    }

    private string GenerateRandomValue(int maxValue)
    {
        if (_includeMaskForIrrelevantValues && _random.Next(ProbabilityToGenerateAMask) == 0)
        {
            return "-";
        }
        
        return _random.Next(maxValue).ToString();
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

    public ActsGenerator IncludeMaskForIrrelevantValues()
    {
        _includeMaskForIrrelevantValues = true;
        return this;
    }
}