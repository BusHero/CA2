using System.IO.Abstractions;

namespace GeneratorLibrary;

public class CsvGeneratorToFile(
    IFileSystem fileSystem,
#pragma warning disable CS9113 // Parameter is unread.
    RandomCsvGenerator csvGenerator)
#pragma warning restore CS9113 // Parameter is unread.
{
    public async Task Generate(
        string destinationFolder, 
        string filename, 
        int rowsCount, 
        string[][] columns)
    {
        fileSystem.Directory.CreateDirectory(destinationFolder);

        var csv = csvGenerator.Generate();
        var content = csv.Select(x => string.Join(',', x));
        
        await fileSystem.File.WriteAllLinesAsync(
            Path.Combine(destinationFolder, filename), 
            content);
    }
}