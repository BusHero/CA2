using System.Numerics;

namespace TestUtils;

public static class UtilsExtensions
{
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