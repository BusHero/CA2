namespace CA2.Tests;

internal static class Extensions
{
    internal static string[,] Pivot(this string[,] csv)
    {
        var result = new string[csv.GetLength(1), csv.GetLength(0)];

        for (var i = 0; i < csv.GetLength(0); i++)
        {
            for (var j = 0; j < csv.GetLength(1); j++)
            {
                result[j, i] = csv[i, j];
            }
        }

        return result;
    }

    internal static string[][] Pivot(this string[][] csv)
    {
        var result = Enumerable
            .Range(0, csv[0].Length)
            .Select(_ => new string[csv.Length])
            .ToArray();

        for (var i = 0; i < csv.Length; i++)
        {
            for (var j = 0; j < csv[0].Length; j++)
            {
                result[j][i] = csv[i][j];
            }
        }

        return result;
    }
}
