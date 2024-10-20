using System.Diagnostics.CodeAnalysis;
using System.Numerics;

namespace TestUtils;

public static class Extensions
{
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    public static T[,] Pivot<T>(this T[,] csv)
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

    public static T[][] Pivot<T>(this T[][] csv)
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

    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    public static string ConvertToString(this string[][] csv)
    {
        var rows = csv.Select(row => row.ConvertToString());

        return string.Join(Environment.NewLine, rows);
    }

    public static string ConvertToString<T>(this IEnumerable<T> csv, string delimiter = ", ")
        => string.Join(delimiter, csv);
    
    public static BigInteger CalculateMaximumNumber(this IEnumerable<int> sizes) =>
        sizes
            .Aggregate(BigInteger.One, (r, x) => r * x);

    public static BigInteger[] CalculateSizes(this int[] sizes)
    {
        switch (sizes.Length)
        {
            case 0:
                return [];
            case 1:
                return [1];
        }

        var result = new BigInteger[sizes.Length];

        result[^1] = 1;

        for (var i = 2; i <= result.Length; i++)
        {
            result[^i] = result[^(i - 1)] * sizes[^(i - 1)];
        }

        return result;
    }

}