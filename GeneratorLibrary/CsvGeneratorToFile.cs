using System.IO.Abstractions;

namespace GeneratorLibrary;

public class CsvGeneratorToFile(
    IFileSystem fileSystem,
    RandomCsvGenerator csvGenerator)
{
    public async Task Generate(
        string destinationFolder, 
        string filename, 
        int rowsCount, 
        string[][] columns)
    {
        fileSystem.Directory.CreateDirectory(destinationFolder);

        var csv = csvGenerator
            .WithRowsCount(rowsCount)
            .Generate();
        
        var content = csv.Select(x => string.Join(',', x));
        
        await fileSystem.File.WriteAllLinesAsync(
            Path.Combine(destinationFolder, filename), 
            content);
    }
}