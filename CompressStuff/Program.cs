using System.IO.Abstractions;
using System.Text.RegularExpressions;

using CA2.Compression;
using CA2.Console;
using CA2.Extractors;

namespace CompressStuff;

internal partial class Program
{
    private const string Path = """C:\Users\Petru\projects\csharp\CA2\result-unarchive""";

    public static async Task Main()
    {
        var files = new DirectoryInfo(Path).GetFiles("*.txt");

        var command = new CompressCommand(
            new FileSystem(),
            [new ActsExtractor(),],
            new Compressor());

        await Parallel.ForEachAsync(files, async (file, _) =>
        {
            var match = Filename().Match(file.Name);
            var t = byte.Parse(match.Groups["t"].Value);
            var k = int.Parse(match.Groups["k"].Value);
            var v = int.Parse(match.Groups["v"].Value);

            var columns = Enumerable
                .Repeat(v, k)
                .ToArray();

            try
            {
                await command.Command("acts", file.FullName, null, columns, t);
                Console.WriteLine($@"✓ {file.Name}");
            }
            catch
            {
                Console.WriteLine($"X - {file.Name}");
            }
        });
    }

    [GeneratedRegex("""ca\.(?<t>\d)\.(?<v>\d)\^(?<k>\d+)\.txt""")]
    private static partial Regex Filename();
}