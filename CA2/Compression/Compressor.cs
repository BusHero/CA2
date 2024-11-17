namespace CA2.Compression;

using System.Numerics;
using System.Text;
using System.Threading.Tasks;

public sealed class Compressor : IDecompressor, ICompressor, IMetadataWriter
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
            throw new InvalidOperationException("Values and Sizes have different length");
        }

        if (row.Any(x => x < 0))
        {
            throw new InvalidOperationException("Values contain elements smaller than zero");
        }

        if (sizes.Any(x => x < 2))
        {
            throw new InvalidOperationException("Sizes contain elements smaller than 2");
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

    private static async Task CompressAsync(int[] combinationItem, int[] combinationSizes, Stream stream)
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
            intermediateResult /= sizes[i];
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

    public async Task CompressAsync(
        int[][] items,
        int[] sizes,
        Stream stream)
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

        Write(
            csv.Length,
            sizes,
            interactionStrength,
            metaStream);

        await Task.CompletedTask;
    }

    public void Write(
        long numberOfRows,
        IReadOnlyCollection<int> columns,
        byte interactionStrength,
        Stream metaStream)
    {
        var writer = new BinaryWriter(metaStream);

        writer.Write(CcaMagicSequenceBytes);
        writer.Write((short)2);
        writer.Write(numberOfRows);
        writer.Write(interactionStrength);

        var groups = columns
            .GroupBy(x => x)
            .OrderByDescending(x => x.Key);

        foreach (var group in groups)
        {
            var length = group.Count();

            switch (length)
            {
                case <= 0x7f:
                    writer.Write((byte)length);
                    break;
                case <= 0x3fff:
                    writer.Write((byte)(length >> 8 | 0x80));
                    writer.Write((byte)(length & 0xff));
                    break;
                case <= 0x1fffff:
                    writer.Write((byte)(length >> 16 | 0xc0));
                    writer.Write((byte)(length >> 8 & 0xff));
                    writer.Write((byte)(length & 0xff));
                    break;
                default:
                    writer.Write((byte)(length >> 24 | 0xe0));
                    writer.Write((byte)(length >> 16 & 0xff));
                    writer.Write((byte)(length >> 8 & 0xff));
                    writer.Write((byte)(length & 0xff));
                    break;
            }

            writer.Write((byte)group.Key);
        }

        writer.Write(ushort.MinValue);
    }
}

public class Compressor2
{
    public async Task CompressAsync(
        int[][] items,
        IReadOnlyCollection<int> sizes,
        Stream stream)
    {
        ArgumentNullException.ThrowIfNull(items);
        ArgumentNullException.ThrowIfNull(sizes);

        var bytesPerCombination = sizes.Product().GetByteCount();

        var bytes = new byte[bytesPerCombination];

        var sortedSizes = sizes
            .OrderDescending()
            .ToArray();
        
        foreach (var item in items)
        {
            var stuff = item
                .Zip(sizes, (i, size) => (item: i, size))
                .OrderByDescending(x => x.size)
                .ToArray();

            var sortedItems = stuff.Select(x => x.item).ToArray();

            BigInteger result = sortedItems[^1];
            var power = BigInteger.One;
            
            for (var i = sortedItems.Length - 2; i >= 0; i--)
            {
                power *= sortedSizes[i + 1];
                result += sortedItems[i] * power;
            }

            result.TryWriteBytes(bytes, out _);

            await stream.WriteAsync(bytes);
        }
    }
}

public static class MyEnumerableExtensions
{
    public static BigInteger Product(this IEnumerable<int> numbers)
        => numbers.Aggregate(BigInteger.One, (x, y) => x * y);
}