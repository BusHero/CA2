namespace CA2.Tests;

using System.Diagnostics.CodeAnalysis;

internal static class Extensions
{
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    internal static T[,] Pivot<T>(this T[,] csv)
    {
        var result = new T[csv.GetLength(1), csv.GetLength(0)];

        for (var i = 0; i < csv.GetLength(0); i++)
        {
            for (var j = 0; j < csv.GetLength(1); j++)
            {
                result[j, i] = csv[i, j];
            }
        }

        return result;
    }

    internal static T[][] Pivot<T>(this T[][] csv)
    {
        var result = Enumerable
            .Range(0, csv[0].Length)
            .Select(_ => new T[csv.Length])
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