using GeneratorLibrary;

namespace CA2.Tests;

internal sealed class FakeFileSystem : IFileSystem
{
    private readonly Dictionary<string, string> _files = [];
    
    public bool ContainsFile(string fileName)
    {
        return _files.ContainsKey(fileName);
    }

    public void Add(string fileName, string content)
    {
        _files[fileName] = content;
    }

    public string? ReadFile(string filename)
    {
        _files.TryGetValue(filename, out var result);
        return result;
    }
}