namespace GeneratorLibrary.CsvGenerators;

public interface IRandomCsvGenerator
{
    string[][] Generate();
    
    IRandomCsvGenerator WithRowsCount(int rows);
    
    IRandomCsvGenerator WithColumn(string[] column);
}