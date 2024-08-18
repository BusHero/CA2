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
        
        var result = BigInteger.Zero;
        
        for (var i = 0; i < values.Length - 1; i++)
        {
            result = result * sizes[i] + values[i] * sizes[i + 1];
        }
        
        return result + values[^1];
    }
}