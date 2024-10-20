namespace CsvGenerator;

public static class CsvRandomGeneratorExtensions
{
    public static IRandomCsvGenerator WithColumn(this IRandomCsvGenerator generator, int numberOfValues)
    {
        var column = Enumerable
            .Range(0, numberOfValues)
            .Select(x => x.ToString())
            .ToArray();

        return generator.WithColumn(column);
    }

    public static IRandomCsvGenerator WithColumns(this IRandomCsvGenerator generator, int[] columns)
        => columns
            .Aggregate(
                generator,
                (gen, column) => gen.WithColumn(column));

    public static IRandomCsvGenerator WithColumns(this IRandomCsvGenerator generator, string[][] columns)
        => columns
            .Aggregate(
                generator,
                (gen, column) => gen.WithColumn(column));
}
