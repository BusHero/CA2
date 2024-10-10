namespace GeneratorLibrary.Compression;

using System.IO;
using System.Numerics;

public interface ICompressor
{
    BigInteger Compress(int[] row, int[] sizes);
    
    void Compress(int[] combinationItem, int[] combinationSizes, Stream stream);
    void Compress(int[][] items, int[] sizes, MemoryStream stream);
    byte[] CompressToBytes(int[] combinationItem, int[] combinationSizes);
}
