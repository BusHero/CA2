namespace CA2.Optimization;

public interface ICsvOptimizer
{
    OptimizedCsv Optimize(string[][] csv);
}