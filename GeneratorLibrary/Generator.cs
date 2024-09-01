using System.Buffers.Binary;
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

        return values
            .Zip(sizes.Skip(1))
            .Reverse()
            .Aggregate(
                (result: (BigInteger)values[^1], power: BigInteger.One),
                (r, t) =>
                {
                    var (result, power) = r;
                    var (value, size) = t;

                    power *= size;
                    result += value * power;

                    return (result, power);
                }).result;
    }

    public static long GetNumberOfBitsFoCombination(int[] sizes)
    {
        return sizes
            .Aggregate(BigInteger.One, (x, y) => x * y)
            .GetBitLength();
    }
}