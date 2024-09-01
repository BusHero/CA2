using System.Numerics;

namespace GeneratorLibrary;

public sealed class Generator
{
    public static BigInteger Generate(
        int[] values,
        int[] sizes)
    {
        ArgumentNullException.ThrowIfNull(values);
        ArgumentNullException.ThrowIfNull(sizes);
        if (values.Length != sizes.Length)
        {
            throw new InvalidOperationException($"Values and Sizes have different length");
        }

        if (values.Any(x => x < 0))
        {
            throw new InvalidOperationException($"Values contain elements smaller than zero");
        }

        if (sizes.Any(x => x < 2))
        {
            throw new InvalidOperationException($"Sizes contain elements smaller than 2");
        }

        if (values
            .Zip(sizes, (value, size) => size <= value)
            .Any(x => x))
        {
            throw new InvalidOperationException("value is bigger than size");
        }

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

    public static long GetNumberOfBitsFoCombination(int[] sizes)
    {
        return sizes
            .Aggregate(BigInteger.One, (x, y) => x * y)
            .GetBitLength();
    }
}