namespace CsvGenerator;

public interface IRandomCsvGenerator
{
    string[][] Generate();

    void Generate(Stream stream);

    IRandomCsvGenerator WithRowsCount(int rows);

    IRandomCsvGenerator WithColumn(string[] column);
}