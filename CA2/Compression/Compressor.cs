namespace CA2.Compression;

using System.Numerics;
using System.Text;
using System.Threading.Tasks;

public sealed class Compressor : IDecompressor, ICompressor
{
    private const string CcaMagicSequence = " CCA";
    private static readonly byte[] CcaMagicSequenceBytes = Encoding.ASCII.GetBytes(CcaMagicSequence);

    public static BigInteger Compress(
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
            .Zip(sizes, (value, size) => size <= value)
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

    public static long GetNumberOfBitsForCombination(int[] sizes)
        => sizes
            .Aggregate(
                BigInteger.One,
                (x, y) => x * y)
            .GetBitLength();

    private static int GetNumberOfBytesForCombination(int[] sizes)
        => sizes
            .Aggregate(
                BigInteger.One,
                (x, y) => x * y)
            .GetByteCount();

    private static async Task TryWriteToBufferAsync(
        Stream stream,
        BigInteger[] numbers,
        int sizeItem)
    {
        if (numbers is [])
        {
            return;
        }

        var byteCount = numbers
            .Max(BigInteger.Abs)
            .GetByteCount();

        if (sizeItem < byteCount)
        {
            return;
        }

        foreach (var number in numbers)
        {
            var bytes = new byte[sizeItem];

            number.TryWriteBytes(
                bytes,
                out _);
            await stream.WriteAsync(bytes);
        }
    }

    private async Task CompressAsync(int[] combinationItem, int[] combinationSizes, Stream stream)
    {
        var number = Compress(combinationItem, combinationSizes);
        var size = GetNumberOfBytesForCombination(combinationSizes);

        await TryWriteToBufferAsync(stream, [number], size);
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

    private async Task CompressAsync(int[][] items, int[] sizes, Stream stream)
    {
        foreach (var item in items)
        {
            await CompressAsync(item, sizes, stream);
        }
    }

    public async Task CompressAsync(
        int[][] csv,
        int[] sizes,
        byte interactionStrength,
        Stream ccaStream,
        Stream metaStream)
    {
        await CompressAsync(
            csv,
            sizes,
            ccaStream);

        WriteMetadata(
            csv.Length,
            sizes.Select(x => (byte)x),
            interactionStrength,
            metaStream);

        await Task.CompletedTask;
    }

    private static void WriteMetadata(
        long numberOfRows,
        IEnumerable<byte> columns,
        byte interactionStrength,
        Stream metaStream)
    {
        var writer = new BinaryWriter(metaStream);

        writer.Write(CcaMagicSequenceBytes);
        writer.Write((short)2);
        writer.Write(numberOfRows);
        writer.Write(interactionStrength);
        foreach (var column in columns)
        {
            writer.Write(column);
        }
        writer.Write(ushort.MinValue);
    }
}