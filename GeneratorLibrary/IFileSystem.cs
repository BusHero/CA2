namespace GeneratorLibrary;

public interface IFileSystem
{
    bool ContainsFile(string outputFile);
    
    void Add(string fileName, string content);
    
    string? ReadFile(string filename);
}