namespace CA2.Extractors;

public sealed class CsvExtractor : IExtractor
{
    public string Format => "CSV";
    
    public async Task<int[][]> ExtractAsync(TextReader reader)
    {
        var csv = await GetCsv(reader);

        var report = Optimize(csv);

        return report;
    }

    private static int[][] Optimize(string[][] csv)
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

        return result;
    }

    private static async Task<string[][]> GetCsv(TextReader reader)
    {
        var text = await reader.ReadToEndAsync();

        var csv = text.Split(Environment.NewLine)
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Select(line => line.Split(',').ToArray())
            .ToArray();

        return csv;
    }
}