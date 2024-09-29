namespace GeneratorLibrary.Compression;

public interface ICsvCompressor
{
    Task CompressAsync(
        string[][] csv, 
        Stream stream, 
        CancellationToken cancellationToken = default);
}