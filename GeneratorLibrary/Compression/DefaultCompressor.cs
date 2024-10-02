namespace GeneratorLibrary.Compression;

public sealed class DefaultCompressor : ICsvCompressor
{
    public Task CompressAsync(
        string[][] csv,
        Stream stream,
        CancellationToken cancellationToken = default)
    {
        stream.Write([0x00]);

        return Task.CompletedTask;
    }
}