namespace CsvGenerator;

public static class CsvGeneratorToFileExtensions
{
    public static Task GenerateAsync(this CsvFileGenerator generator, Stream writer, int rowsCount, string[][] columns)
        => generator.GenerateAsync(new StreamWriter(writer), rowsCount, columns);
}
