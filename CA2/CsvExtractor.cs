namespace CA2;

public sealed class CsvExtractor : IExtractor
{
    public Task<int[][]> ExtractAsync(Stream stream)
        => Task.FromResult(Array.Empty<int[]>());
}