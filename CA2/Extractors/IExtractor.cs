namespace CA2.Extractors;

public interface IExtractor
{
    string Format { get; }
    
    Task<int[][]> ExtractAsync(TextReader reader);
}
