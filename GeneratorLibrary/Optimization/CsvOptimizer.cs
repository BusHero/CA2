namespace GeneratorLibrary.Optimization;

public static class CsvOptimizer
{
    public static OptimizedCsv Optimize(string[][] csv)
    {
        var values = Enumerable
            .Range(0, csv[0].Length)
            .Select(_ => new List<string>())
            .ToList();

        foreach (var t in csv)
        {
            for (var j = 0; j < t.Length; j++)
            {
                var indexOf = values[j].IndexOf(t[j]);

                if (indexOf != -1)
                {
                    continue;
                }

                values[j].Add(t[j]);
            }
        }

        var result = Enumerable
            .Range(0, csv.Length)
            .Select(_ => new int[csv[0].Length])
            .ToArray();

        for (var i = 0; i < csv.Length; i++)
        {
            for (var j = 0; j < csv[i].Length; j++)
            {
                result[i][j] = values[j].IndexOf(csv[i][j]);
            }
        }

        return new OptimizedCsv(
            result,
            values);
    }
}