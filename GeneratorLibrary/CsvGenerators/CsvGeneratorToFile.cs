using System.IO.Abstractions;

namespace GeneratorLibrary.CsvGenerators;

public class CsvGeneratorToFile(
    IFileSystem fileSystem,
    IRandomCsvGeneratorFactory factory)
{
    public async Task GenerateAsync(
        string destinationFolder, 
        string filename, 
        int rowsCount, 
        string[][] columns)
    {
        fileSystem.Directory.CreateDirectory(destinationFolder);

        var csv = factory
            .Create()
            .WithRowsCount(rowsCount)
            .WithColumns(columns)
            .Generate();
        
        var content = csv.Select(x => string.Join(',', x));
        
        await fileSystem.File.WriteAllLinesAsync(
            Path.Combine(destinationFolder, $"{filename}.csv"), 
            content);
    }
}