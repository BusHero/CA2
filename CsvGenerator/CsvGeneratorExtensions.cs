namespace CsvGenerator;

using System.Threading.Tasks;

public static class CsvGeneratorExtensions
{
    public static ICsvGenerator WithColumn(this ICsvGenerator generator, int numberOfValues)
    {
        var column = Enumerable
            .Range(0, numberOfValues)
            .Select(x => x.ToString())
            .ToArray();

        return generator.WithColumn(column);
    }

    public static ICsvGenerator WithColumns(this ICsvGenerator generator, int[] columns)
        => columns
            .Aggregate(
                generator,
                (gen, column) => gen.WithColumn(column));

    public static ICsvGenerator WithColumns(this ICsvGenerator generator, string[][] columns)
        => columns
            .Aggregate(
                generator,
                (gen, column) => gen.WithColumn(column));

    public static async Task GenerateAsync(
        this ICsvGenerator generator, 
        TextWriter writer,
        CancellationToken token = default)
    {
        var csv = generator
            .Generate()
            .Select(row => string.Join(',', row));

        var content = string.Join(Environment.NewLine, csv);
        await writer.WriteAsync(content);
        await writer.FlushAsync(token);
    }
}
