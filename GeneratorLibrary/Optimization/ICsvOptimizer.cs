namespace GeneratorLibrary.Optimization;

public interface ICsvOptimizer
{
    CsvOptimizer.OptimizationReport Optimize(string[][] csv);
}