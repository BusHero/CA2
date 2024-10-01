namespace GeneratorLibrary.Optimization;

public interface ICsvOptimizer
{
    OptimizedCsv Optimize(string[][] csv);
}