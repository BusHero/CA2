namespace GeneratorLibrary.Optimization;

public sealed record OptimizedCsv(
    int[][] Csv,
    List<List<string>> ValuesMap)
{
    public int[] Sizes { get; } = ValuesMap
        .Select(x => x.Count)
        .ToArray();
}