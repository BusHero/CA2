namespace CsvGenerator;

public interface IRandomCsvGenerator
{
    IEnumerable<string[]> Generate();

    IRandomCsvGenerator WithRowsCount(int rows);

    IRandomCsvGenerator WithColumn(string[] column);
}