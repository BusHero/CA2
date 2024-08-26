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
        var power = BigInteger.One;
        
        for (var i = 1; i < sizes.Length; i++)
        {
            power *= sizes[i - 1];
            result += values[i] * power;
        }
        
        return result;
    }
}