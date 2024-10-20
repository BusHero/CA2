namespace CsvGenerator;

public static class CsvGeneratorExtensions
{
    public static Task GenerateAsync(this ICsvGenerator generator, Stream writer, int rowsCount, string[][] columns)
        => generator.GenerateAsync(new StreamWriter(writer), rowsCount, columns);
}
