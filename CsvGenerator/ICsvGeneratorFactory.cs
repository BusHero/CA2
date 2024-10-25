namespace CsvGenerator;

public interface ICsvGeneratorFactory
{
    ICsvGenerator Create();

    ICsvGenerator Create(int seed);
}