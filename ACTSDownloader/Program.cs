using System.Diagnostics.CodeAnalysis;
using System.Web;

var directory = """..\..\..\result""";
Directory.CreateDirectory(directory);


var client = new HttpClient()
{
    BaseAddress = new Uri("https://math.nist.gov/coveringarrays/ipof/cas/"),
};

await SaveFile(client, directory, 2, 2, 3);
await SaveFile(client, directory, 2, 2, 4);

return;

static async Task SaveFile(
    HttpClient httpClient, 
    string directory,
    int t, 
    int v, 
    int k)
{
    var filename = GetFilename(t, v, k);
    var response = await httpClient.GetAsync($"t={t}/v={v}/{HttpUtility.UrlEncode(filename)}");
    response.EnsureSuccessStatusCode();
    await using var stream = await response.Content.ReadAsStreamAsync();
    await using var file = File.OpenWrite(Path.Combine(directory, filename));
    await stream.CopyToAsync(file);
}

static string GetFilename(int t, int v, int k)
{
    return $"ca.{t}.{v}^{k}.txt.zip";
}

[SuppressMessage("ReSharper", "UnusedType.Global")]
public record Combination(
    int T,
    int V,
    int MinK,
    int MaxK);