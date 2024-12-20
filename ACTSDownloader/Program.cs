﻿using System.Web;

const string directory = """..\..\..\result""";
Directory.CreateDirectory(directory);

Combination[] combinations =
[
    new(2, 2, 3, 2000),
    new(3, 2, 4, 2000),
    new(4, 2, 5, 909),
    new(5, 2, 6, 191),
    new(6, 2, 7, 86),

    new(2, 3, 3, 2000),
    new(3, 3, 4, 2000),
    new(4, 3, 5, 500),
    new(5, 3, 6, 116),
    new(6, 3, 7, 51),

    new(2, 4, 3, 2000),
    new(3, 4, 4, 2000),
    new(4, 4, 5, 308),
    new(5, 4, 6, 81),
    new(6, 4, 7, 36),

    new(2, 5, 3, 2000),
    new(3, 5, 4, 1971),
    new(4, 5, 5, 208),
    new(5, 5, 6, 61),
    new(6, 5, 7, 25),

    new(2, 6, 3, 2000),
    new(3, 6, 4, 1306),
    new(4, 6, 5, 163),
    new(5, 6, 6, 46),
];

var client = new HttpClient
{
    BaseAddress = new Uri("https://math.nist.gov/coveringarrays/ipof/cas/"),
};

var values = combinations
    .SelectMany(combination => Enumerable
        .Range(combination.MinK, combination.MaxK - combination.MinK + 1)
        .Select(k => (combination.T, combination.V, k)));

await Parallel.ForEachAsync(values, Body);

return;

async ValueTask Body((int T, int V, int k) tuple, CancellationToken token)
{
    var (t, v, k) = tuple;
    var filename = GetFilename(t, v, k);
    var response = await client.GetAsync($"t={t}/v={v}/{HttpUtility.UrlEncode(filename)}", token);

    if (!response.IsSuccessStatusCode)
    {
        Console.WriteLine($"X - {filename}");
    }
    else
    {
        await using var stream = await response.Content.ReadAsStreamAsync(token);
        await using var file = File.OpenWrite(Path.Combine(directory, filename));
        await stream.CopyToAsync(file, token);
        Console.WriteLine($"✓ - {filename}");
    }
}

static string GetFilename(int t, int v, int k)
    => $"ca.{t}.{v}^{k}.txt.zip";

internal sealed record Combination(
    int T,
    int V,
    int MinK,
    int MaxK);