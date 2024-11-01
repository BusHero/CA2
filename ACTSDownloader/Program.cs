using System.Web;

using var client = new HttpClient();
client.BaseAddress = new Uri("https://math.nist.gov/coveringarrays/ipof/cas");

var directory = "result";
Directory.CreateDirectory(directory);

Combination[] combinations =
[
     new Combination(2, 2, 3, 10),
    // new Combination(2, 2, 3, 2000),
    // new Combination(3, 2, 4, 2000),
    // new Combination(4, 2, 5, 909),
    // new Combination(5, 2, 6, 191),
    // new Combination(6, 2, 7, 86),
    //
    // new Combination(2, 3, 3, 2000),
    // new Combination(3, 3, 4, 2000),
    // new Combination(4, 3, 5, 500),
    // new Combination(5, 3, 6, 116),
    // new Combination(6, 3, 7, 51),
    //
    // new Combination(2, 4, 3, 2000),
    // new Combination(3, 4, 4, 2000),
    // new Combination(4, 4, 5, 308),
    // new Combination(5, 4, 6, 81),
    // new Combination(6, 4, 7, 36),
    //
    // new Combination(2, 5, 3, 2000),
    // new Combination(3, 5, 4, 1971),
    // new Combination(4, 5, 5, 208),
    // new Combination(5, 5, 6, 61),
    // new Combination(6, 5, 7, 25),
    //
    // new Combination(2, 6, 3, 2000),
    // new Combination(3, 6, 4, 1306),
    // new Combination(4, 6, 5, 163),
    // new Combination(5, 6, 6, 46),
];

var foo = combinations
    .SelectMany(x => Enumerable
        .Range(x.MinK, x.MaxK - x.MinK)
        .Select(k => (x.T, x.V, k)));

await Parallel.ForEachAsync(
    foo, async
        (x, token) =>
    {
        
        await SaveFile(x.T, x.V, x.k, client, directory, token);
    });
Console.WriteLine("Done");

var client2 = new HttpClient();
var response = await client2.GetAsync("https://math.nist.gov/coveringarrays/ipof/cas/t=2/v=2/ca.2.2%5E3.txt.zip");
response.EnsureSuccessStatusCode();
await using var stream = await response.Content.ReadAsStreamAsync();
await using var file = File.OpenWrite("test.zip");
await stream.CopyToAsync(file);

return;

static async Task SaveFile(int t, int v, int k, HttpClient httpClient, string directory, CancellationToken token)
{
    var filename = GetFilename(t, v, k);
    var urlEncoded = HttpUtility.UrlEncode(filename);
    var path = $"/t={t}/v={v}/{urlEncoded}";
    var response = await httpClient.GetAsync(path, token);
    var uri = new UriBuilder(httpClient.BaseAddress!)
    {
        Path = path,
    }.Uri;

    Console.WriteLine(uri);
    
    if (!response.IsSuccessStatusCode)
    {
        Console.WriteLine($"Failed to save file {filename}");
    }
    
    var content = await response.Content.ReadAsStreamAsync(token);
    
    var file = Path.Combine(directory, filename);
    await using var stream = File.OpenWrite(file);
    content.CopyTo(stream);
    
    Console.WriteLine($"Done {file}");
}

string GetFilename(int t, int v, int i)
{
    return $"ca.{t}.{v}^{i}.txt.zip";
}

record Combination(
    int T,
    int V,
    int MinK,
    int MaxK);