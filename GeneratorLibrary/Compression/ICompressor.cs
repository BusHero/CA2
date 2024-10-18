namespace GeneratorLibrary.Compression;

using System.IO;

public interface ICompressor
{
    Task CompressAsync(string[][] csv, int[] sizes, Stream stream);
    Task CompressAsync(string[][] csv, int[] columns, byte interactionStrength, Stream ccaStream, Stream metaStream);
}
