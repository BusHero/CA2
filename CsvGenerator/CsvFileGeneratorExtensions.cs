namespace CsvGenerator;

public static class CsvFileGeneratorExtensions
{
    public static Task GenerateAsync(this CsvFileGenerator generator, Stream writer, int rowsCount, string[][] columns)
        => generator.GenerateAsync(new StreamWriter(writer), rowsCount, columns);
}
