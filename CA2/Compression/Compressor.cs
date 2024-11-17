namespace CA2.Compression;

using System.Numerics;
using System.Text;
using System.Threading.Tasks;

public class Compressor : ICompressor
{
    private const string CcaMagicSequence = " CCA";
    private static readonly byte[] CcaMagicSequenceBytes = Encoding.ASCII.GetBytes(CcaMagicSequence);
    
    public async Task WriteCsvAsync(
        int[][] items,
        IReadOnlyCollection<int> sizes,
        Stream stream,
        CancellationToken token = default)
    {
        ArgumentNullException.ThrowIfNull(items);
        ArgumentNullException.ThrowIfNull(sizes);

        var bytesPerCombination = sizes
            .Product()
            .GetByteCount();

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

            var sortedItems = stuff
                .Select(x => x.item)
                .ToArray();

            BigInteger result = sortedItems[^1];
            var power = BigInteger.One;

            for (var i = sortedItems.Length - 2; i >= 0; i--)
            {
                power *= sortedSizes[i + 1];
                result += sortedItems[i] * power;
            }

            result.TryWriteBytes(bytes, out _);

            await stream.WriteAsync(bytes, token);
        }
    }
    
    public void WriteMetadata(
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

public static class MyEnumerableExtensions
{
    public static BigInteger Product(this IEnumerable<int> numbers)
        => numbers.Aggregate(BigInteger.One, (x, y) => x * y);
}