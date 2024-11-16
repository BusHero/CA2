namespace CA2.Tests.Compression;

public static class ColumnsExtractor
{
    public static byte[] GetColumns(byte[] bytes)
    {
        var result = Enumerable.Empty<byte>();

        while (bytes.Length != 0)
        {
            var length = GetValue(bytes);
            var numberOfBytes = CountNecessaryBytesStoreLength(length);

            var sequence = Enumerable.Repeat(bytes[numberOfBytes], length);
            result = result.Concat(sequence);
            bytes = bytes[(numberOfBytes + 1)..];
        }

        return result.ToArray();
    }

    private static int CountNecessaryBytesStoreLength(int x) => x switch
    {
        <= 0x7f => 1,
        <= 0x3fff => 2,
        <= 0x1fffff => 3,
        <= 0x0fffffff => 4,
        _ => throw new InvalidOperationException(),
    };

    internal static byte[] GetBytes(int x)
        => x switch
        {
            <= 0x7f => [(byte)x],
            <= 0x3fff => [(byte)(x >> 8 | 0x80), (byte)(x & 0xff)],
            <= 0x1fffff => [(byte)(x >> 16 | 0xc0), (byte)(x >> 8 & 0xff), (byte)(x & 0xff)],
            <= 0x0fffffff => [(byte)(x >> 24 | 0xe0), (byte)(x >> 16 & 0xff), (byte)(x >> 8 & 0xff), (byte)(x & 0xff)],
            _ => throw new InvalidOperationException(),
        };

    internal static int GetValue(byte[] bytes)
        => bytes[0] switch
        {
            var b when (b & 0x80) == 0x00 => bytes[0],
            var b when (b & 0xc0) == 0x80 => (bytes[0] & 0x7f) << 8 | bytes[1],
            var b when (b & 0xe0) == 0xc0 => (bytes[0] & 0x3f) << 16 | bytes[1] << 8 | bytes[2],
            var b when (b & 0xf0) == 0xe0 => (bytes[0] & 0x0f) << 24 | bytes[1] << 16 | bytes[2] << 8 | bytes[3],
            _ => throw new InvalidOperationException(),
        };

    public static int GetNumberOfBytesFromMask(byte @byte)
        => @byte switch
        {
            _ when (@byte & 0x80) == 0x00 => 1,
            _ when (@byte & 0xc0) == 0x80 => 2,
            _ when (@byte & 0xe0) == 0xc0 => 3,
            _ when (@byte & 0xf0) == 0xe0 => 4,
            _ => throw new InvalidOperationException(),
        };
}
