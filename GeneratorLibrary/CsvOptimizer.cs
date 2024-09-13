namespace GeneratorLibrary;

public static class CsvOptimizer
{
    public static int[][] Optimize(string[][] csv)
    {
        var counter = 0;
        var dict = new Dictionary<string, int>();

        return csv
            .Select(x =>
            {
                if (!dict.TryGetValue(x[0], out var value))
                {
                    value = counter;
                    dict[x[0]] = value;
                    counter++;
                }

                return new[] { value };
            })
            .ToArray();
    }
}