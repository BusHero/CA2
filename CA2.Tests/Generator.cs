using System.Numerics;

namespace CA2.Tests;

public sealed class Generator
{
    public static BigInteger Generate(int[] values, int[] sizes)
    {
        if (values.Length == 0)
        {
            return 0;
        }

        sizes = sizes.Reverse().ToArray();
        values = values.Reverse().ToArray();
        BigInteger result = values[0];

        for (var i = 1; i < sizes.Length; i++)
        {
            var foo = sizes.Take(i).Aggregate(BigInteger.One, (a, b) => a * b);
            result += values[i] * foo;
        }
        
        return result;
    }
}