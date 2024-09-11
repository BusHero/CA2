namespace GeneratorLibrary;

using System;

public static class CsvOptimizer
{
    public static CsvOptimizationResult Optimize(string[][] items)
    {
        return new CsvOptimizationResult
        {
            Rows = Enumerable.Range(0, items.Length).ToArray(),
        };
    }
}

public sealed class CsvOptimizationResult
{
    public int[] Rows { get; set; } = null!;
}