namespace CA2.Compression;

using System.IO;

public interface ICompressor
{
    Task CompressAsync(
        int[][] csv,
        int[] sizes,
        byte interactionStrength,
        Stream ccaStream,
        Stream metaStream);
}
