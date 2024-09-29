namespace GeneratorLibrary.CsvGenerators;

public sealed class DefaultRandomCsvGeneratorFactory : IRandomCsvGeneratorFactory
{
    public IRandomCsvGenerator Create()
        => new RandomCsvGenerator();
}