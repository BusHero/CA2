namespace CsvGenerator;

public interface ICsvGenerator
{
    IEnumerable<string[]> Generate();

    ICsvGenerator WithRowsCount(int rows);

    ICsvGenerator WithColumn(string[] column);
}