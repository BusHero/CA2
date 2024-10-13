namespace GeneratorLibrary.Compression;

public interface ICsvCompressor
{
    Task CompressAsync(
        string[][] csv, 
        int[] sizes,
        Stream stream, 
        CancellationToken cancellationToken = default);
}