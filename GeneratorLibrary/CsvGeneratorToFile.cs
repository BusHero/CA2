using System.IO.Abstractions;

namespace GeneratorLibrary;

public class CsvGeneratorToFile(
    IFileSystem fileSystem)
{
    public async Task Generate(
        string destinationFolder, 
        string filename, 
        int rowsCount, 
        string[][] columns)
    {
        fileSystem.Directory.CreateDirectory(destinationFolder);

        await fileSystem.File.WriteAllTextAsync(
            Path.Combine(destinationFolder, filename), 
            string.Empty);
    }
}