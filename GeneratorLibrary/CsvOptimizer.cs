namespace GeneratorLibrary;

public static class CsvOptimizer
{
    public static OptimizationReport Optimize(string[][] csv)
    {
        var dictionaries = Enumerable
            .Range(0, csv[0].Length)
            .Select(_ => new Dictionary<string, int>())
            .ToList();

        foreach (var t in csv)
        {
            for (var j = 0; j < t.Length; j++)
            {
                if (dictionaries[j]
                    .TryGetValue(t[j], out _))
                {
                    continue;
                }

                if (dictionaries[j].Count == 0)
                {
                    dictionaries[j][t[j]] = 0;
                }
                else
                {
                    var max = dictionaries[j]
                        .Values
                        .Max();
                    dictionaries[j][t[j]] = max + 1;
                }
            }
        }

        var result = Enumerable
            .Range(
                0,
                csv.Length)
            .Select(_ => new int[csv[0].Length])
            .ToArray();

        for (var i = 0; i < csv.Length; i++)
        {
            for (var j = 0; j < csv[i].Length; j++)
            {
                result[i][j] = dictionaries[j][csv[i][j]];
            }
        }

        return new OptimizationReport(
            result,
            dictionaries);
    }

    public sealed record OptimizationReport(
        int[][] Csv,
        List<Dictionary<string, int>> ValuesMap);
}