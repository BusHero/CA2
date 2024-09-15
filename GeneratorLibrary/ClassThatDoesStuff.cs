namespace GeneratorLibrary;

public class ClassThatDoesStuff(
    IFileSystem fileSystem)
{
    public void DoStuff(string inputFile)
    {
        var result = Path.GetFileNameWithoutExtension(inputFile);
        
        fileSystem.Add($"{result}.cca", string.Empty);
    }
}