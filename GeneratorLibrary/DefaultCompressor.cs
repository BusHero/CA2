namespace GeneratorLibrary;

public sealed class DefaultCompressor : ICsvCompressor
{
    public Task CompressAsync(
        string[][] csv, 
        Stream stream, 
        CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}