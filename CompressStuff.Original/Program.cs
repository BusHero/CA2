using System.Text.RegularExpressions;

using CliWrap;

namespace CompressStuff.Original;

internal partial class Program
{
    private const string OriginalPath = """C:\Users\Petru\projects\csharp\CA2\result-unarchive""";
    private const string PathToMoveTo = """C:\Users\Petru\projects\csharp\CA2\result-origin""";

    public static async Task Main()
    {
        var files = new DirectoryInfo(OriginalPath)
            .EnumerateFiles("*.csv");
        
        await Parallel.ForEachAsync(files, Body);
    }

    private static async ValueTask Body(FileInfo file, CancellationToken token)
    {
        var match = Filename().Match(file.Name);
        var t = match.Groups["t"].Value;
        var k = int.Parse(match.Groups["k"].Value);
        var v = match.Groups["v"].Value;

        var columns = new List<string>(k * 2);

        for (var i = 0; i < k; i++)
        {
            columns.Add("-v");
            columns.Add(v);
        }

        try
        {
            await Cli.Wrap("""C:\Users\Petru\projects\rust\ca2\target\release\cca.exe""")
                .WithArguments(args => args.Add("--no-header")
                    .Add("--ca")
                    .Add(file.FullName)
                    .Add("-t")
                    .Add(t)
                    .Add(columns))
                .WithStandardOutputPipe(PipeTarget.ToDelegate(Console.WriteLine))
                .WithStandardErrorPipe(PipeTarget.ToDelegate(Console.WriteLine))
                .ExecuteAsync(token);

            var ccaFilename = Path.ChangeExtension(file.Name, "cca");
            File.Move(Path.Combine(OriginalPath, ccaFilename), Path.Combine(PathToMoveTo, ccaFilename));

            var ccmetaFilename = Path.ChangeExtension(file.Name, "ccmeta");
            File.Move(Path.Combine(OriginalPath, ccmetaFilename), Path.Combine(PathToMoveTo, ccmetaFilename));

            Console.WriteLine($@"✓ {file.Name}");
        }
        catch
        {
            Console.WriteLine($"X - {file.Name}");
        }
    }

    [GeneratedRegex("""ca\.(?<t>\d)\.(?<v>\d)\^(?<k>\d+)\.csv""", RegexOptions.Compiled)]
    private static partial Regex Filename();
}