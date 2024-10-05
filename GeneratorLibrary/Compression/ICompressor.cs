namespace GeneratorLibrary.Compression;

using System.Numerics;

public interface ICompressor
{
    BigInteger Compress(int[] row, int[] sizes);
    byte[] CompressToBytes(int[] combinationItem, int[] combinationSizes);
}
