using System.Diagnostics.CodeAnalysis;

using GeneratorLibrary.Optimization;

namespace GeneratorLibrary.Compression;

using System.Numerics;

public sealed class Compressor : IDecompressor, ICompressor
{
    public BigInteger Compress(
        int[] row,
        int[] sizes)
    {
        ArgumentNullException.ThrowIfNull(row);
        ArgumentNullException.ThrowIfNull(sizes);

        if (row.Length != sizes.Length)
        {
            throw new InvalidOperationException($"Values and Sizes have different length");
        }

        if (row.Any(x => x < 0))
        {
            throw new InvalidOperationException($"Values contain elements smaller than zero");
        }

        if (sizes.Any(x => x < 2))
        {
            throw new InvalidOperationException($"Sizes contain elements smaller than 2");
        }

        if (row
            .Zip(
                sizes,
                (value, size) => size <= value)
            .Any(x => x))
        {
            throw new InvalidOperationException("value is bigger than size");
        }

        if (row.Length == 0)
        {
            return 0;
        }

        return row
            .Zip(sizes.Skip(1))
            .Reverse()
            .Aggregate(
                (result: (BigInteger)row[^1], power: BigInteger.One),
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

    public long GetNumberOfBitsForCombination(int[] sizes)
        => sizes
            .Aggregate(
                BigInteger.One,
                (x, y) => x * y)
            .GetBitLength();

    private int GetNumberOfBytesForCombination(int[] sizes)
        => sizes
            .Aggregate(
                BigInteger.One,
                (x, y) => x * y)
            .GetByteCount();

    public byte[] CompressToBytes(int[] combinationItem, int[] combinationSizes)
    {
        var buffer = new byte[GetNumberOfBytesForCombination(combinationSizes)];

        Compress(
                combinationItem,
                combinationSizes)
            .TryWriteBytes(
                buffer,
                out _);

        return buffer;
    }

    public bool TryWriteToBuffer(
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

    [SuppressMessage("ReSharper", "UnusedMember.Global")]
#pragma warning disable IDE0060 // Remove unused parameter
    public BigInteger Compress(OptimizedCsv report)
#pragma warning restore IDE0060 // Remove unused parameter
    {
        return BigInteger.One;
    }

    public void Compress(int[] combinationItem, int[] combinationSizes, Stream stream)
    {
        var number = Compress(combinationItem, combinationSizes);
        var size = GetNumberOfBytesForCombination(combinationSizes);

        TryWriteToBuffer(stream, [number], size);
    }

    public int[] Decompress(BigInteger compressedValue, int[] sizes)
    {
        var result = new int[sizes.Length];
        var intermediateResult = compressedValue;
        for (var i = sizes.Length - 1; i >= 0; i--)
        {
            result[i] = (int)(intermediateResult % sizes[i]);
            intermediateResult = intermediateResult / sizes[i];
        }

        return result;
    }

    public int[] Decompress(byte[] bytes, int[] sizes)
    {
        var number = new BigInteger(bytes, isUnsigned: true);

        return Decompress(number, sizes);
    }

    public int[][] Decompress(int[] sizes, Stream stream)
    {
        var count = GetNumberOfBytesForCombination(sizes);
        
        var bytes = new byte[count];
        var result = new List<int[]>();
        while (stream.Read(bytes, 0, bytes.Length) > 0)
        {
            result.Add(Decompress(bytes, sizes));
        }

        return [.. result];
    }

    public void Compress(int[][] items, int[] sizes, MemoryStream stream)
    {
        foreach(var item in items)
        {
            Compress(item, sizes, stream);
        }
    }
}