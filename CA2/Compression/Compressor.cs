namespace CA2.Compression;

using System.Numerics;
using System.Text;
using System.Threading.Tasks;

public class Compressor : ICompressor
{
    private const string CcaMagicSequence = " CCA";
    private static readonly byte[] CcaMagicSequenceBytes = Encoding.ASCII.GetBytes(CcaMagicSequence);
    
    public async Task WriteCcaAsync(
        int[][] items,
        IReadOnlyCollection<int> sizes,
        Stream stream,
        CancellationToken token = default)
    {
        ArgumentNullException.ThrowIfNull(items);
        ArgumentNullException.ThrowIfNull(sizes);

        var bytesPerCombination = sizes
            .Aggregate(BigInteger.One, (x, y) => x * y)
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
            .OrderByDescending(x => x.Key)
            .Select(x => (count: x.Count(), (byte)x.Key));

        foreach (var (count, key) in groups)
        {
            switch (count)
            {
                case <= 0x7f:
                    writer.Write((byte)count);
                    break;
                case <= 0x3fff:
                    writer.Write((byte)(count >> 8 | 0x80));
                    writer.Write((byte)(count & 0xff));
                    break;
                case <= 0x1fffff:
                    writer.Write((byte)(count >> 16 | 0xc0));
                    writer.Write((byte)(count >> 8 & 0xff));
                    writer.Write((byte)(count & 0xff));
                    break;
                default:
                    writer.Write((byte)(count >> 24 | 0xe0));
                    writer.Write((byte)(count >> 16 & 0xff));
                    writer.Write((byte)(count >> 8 & 0xff));
                    writer.Write((byte)(count & 0xff));
                    break;
            }

            writer.Write(key);
        }

        writer.Write(ushort.MinValue);
    }

}
