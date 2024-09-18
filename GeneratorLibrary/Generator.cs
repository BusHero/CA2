namespace GeneratorLibrary;

using System.Numerics;

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
            .Zip(
                sizes,
                (value, size) => size <= value)
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
                })
            .result;
    }

    public static long GetNumberOfBitsForCombination(int[] sizes)
        => sizes
            .Aggregate(
                BigInteger.One,
                (x, y) => x * y)
            .GetBitLength();

    public static int GetNumberOfBytesForCombination(int[] sizes)
        => sizes
            .Aggregate(
                BigInteger.One,
                (x, y) => x * y)
            .GetByteCount();

    public static byte[] GetBytes(int[] combinationItem, int[] combinationSizes)
    {
        var buffer = new byte[GetNumberOfBytesForCombination(combinationSizes)];

        Generate(
                combinationItem,
                combinationSizes)
            .TryWriteBytes(
                buffer,
                out _);

        return buffer;
    }

    public static bool TryWriteToBuffer(
        Stream stream,
        BigInteger[] numbers,
        int sizeItem)
    {
        var writer = new BinaryWriter(stream);

        if (numbers is [])
        {
            return true;
        }

        var byteCount = numbers
            .Max(BigInteger.Abs)
            .GetByteCount();

        if (sizeItem < byteCount)
        {
            return false;
        }

        foreach (var number in numbers)
        {
            var bytes = new byte[sizeItem];

            number.TryWriteBytes(
                bytes,
                out _);

            writer.Write(bytes);
        }

        return true;
    }
}