using System.Text.RegularExpressions;

using CliWrap;

namespace CompressStuff.Original;

internal partial class Program
{
    private const string Path = """C:\Users\Petru\projects\csharp\CA2\result-unarchive""";

    public static async Task Main()
    {
        var files = new DirectoryInfo(Path).GetFiles("*.csv");

        await Parallel.ForEachAsync(files, async (file, token) =>
        {
            var match = Filename().Match(file.Name);
            var t = byte.Parse(match.Groups["t"].Value);
            var k = int.Parse(match.Groups["k"].Value);
            var v = int.Parse(match.Groups["v"].Value);

            var columns = Enumerable
                .Range(0, k)
                .SelectMany(_ => new[] { "-v", v.ToString() })
                .ToArray();

            try
            {
                await Cli.Wrap("""C:\Users\Petru\projects\rust\ca2\target\release\cca.exe""")
                    .WithArguments(args => args
                        .Add("--no-header")
                        .Add("--ca").Add(file.FullName)
                        .Add("-t").Add(t)
                        .Add(columns))
                    .WithStandardOutputPipe(PipeTarget.ToDelegate(Console.WriteLine))
                    .WithStandardErrorPipe(PipeTarget.ToDelegate(Console.WriteLine))
                    .ExecuteAsync(token);

                Console.WriteLine($@"✓ {file.Name}");
            }
            catch
            {
                Console.WriteLine($"X - {file.Name}");
            }
        });
    }

    [GeneratedRegex("""ca\.(?<t>\d)\.(?<v>\d)\^(?<k>\d+)\.csv""")]
    private static partial Regex Filename();
}