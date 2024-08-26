using System.Numerics;

namespace CA2.Tests;

public sealed class Generator
{
    public static BigInteger Generate(
        int[] values,
        int[] sizes)
    {
        if (values.Length == 0)
        {
            return 0;
        }

        BigInteger result = values[^1];
        var power = BigInteger.One;

        for (var i = sizes.Length - 2; 0 <= i; i--)
        {
            power *= sizes[i + 1];
            result += values[i] * power;
        }

        return result;
    }
}