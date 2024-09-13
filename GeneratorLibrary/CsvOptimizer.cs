namespace GeneratorLibrary;

public static class CsvOptimizer
{
    public static int[][] Optimize(string[][] csv)
    {
        var dicts = Enumerable
            .Range(0, csv[0].Length)
            .Select(_ => new Dictionary<string, int>())
            .ToList();

        for (var i = 0; i < csv.Length; i++)
        {
            for (var j = 0; j < csv[i].Length; j++)
            {
                if (!dicts[j].TryGetValue(csv[i][j], out var value))
                {
                    if (dicts[j].Count == 0)
                    {
                        dicts[j][csv[i][j]] = 0;
                    }
                    else
                    {
                        var max = dicts[j].Values.Max();
                        dicts[j][csv[i][j]] = max + 1;
                    }
                }
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
                result[i][j] = dicts[j][csv[i][j]];
            }
        }

        return result;
    }
}