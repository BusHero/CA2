namespace CA2.Tests.Extractors;

using System.IO;
using System.Threading.Tasks;

using CA2.Extractors;

internal class ActsExtractor : IExtractor
{
    public Task<int[][]> ExtractAsync(TextReader reader)
    {
        int[][] result = [

        ];

        return Task.FromResult(result);
    }
}