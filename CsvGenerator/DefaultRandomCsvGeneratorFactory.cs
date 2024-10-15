namespace CsvGenerator;

public sealed class DefaultRandomCsvGeneratorFactory : IRandomCsvGeneratorFactory
{
    public IRandomCsvGenerator Create()
        => new RandomCsvGenerator();
}