namespace CA2.Extractors;

public sealed class ActsExtractor : IExtractor
{
    public string Format => "Acts";

    public async Task<int[][]> ExtractAsync(TextReader reader)
    {
        var rows = int.Parse(await reader.ReadLineAsync() ?? string.Empty);
        var acts = new List<int[]>();
        for (var i = 0; i < rows; i++)
        {
            var line = await reader.ReadLineAsync();

            var row = line!
                .Trim()
                .Split(' ')
                .Select(x => x == "-" ? 0 : int.Parse(x))
                .ToArray();
            
            acts.Add(row);
        }
        
        return acts.ToArray();
    }
}