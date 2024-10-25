namespace CA2.Extractors;

public interface IExtractor
{
    Task<int[][]> ExtractAsync(TextReader reader);
}
