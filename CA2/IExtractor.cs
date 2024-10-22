namespace CA2;

public interface IExtractor
{
    Task<int[][]> ExtractAsync(Stream stream);
}