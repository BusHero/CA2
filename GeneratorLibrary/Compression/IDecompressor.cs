using System.Numerics;

namespace GeneratorLibrary.Compression;

public interface IDecompressor
{
    int[] Decompress(BigInteger compressedNumber, int[] sizes);
    
    int[] Decompress(byte[] bytes, int[] sizes);

    int[][] Decompress(int[] sizes, Stream stream);
}