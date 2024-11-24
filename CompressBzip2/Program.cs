using System.IO.Compression;
using System.Text.RegularExpressions;

using CliWrap;

namespace CompressBzip2;

internal partial class Program
{
    private const string PathToOriginalFiles = "/mnt/c/Users/Petru/projects/csharp/CA2/result-unarchive";
    private const string PathToFolderContainingBZips = "/mnt/c/Users/Petru/projects/csharp/CA2/result-bzip2";

    public static async Task Main()
    {
        Directory.CreateDirectory(PathToFolderContainingBZips);
        var directory = new DirectoryInfo(PathToOriginalFiles);

        var files = directory
            .EnumerateFiles("*.txt");

        await Parallel.ForEachAsync(files, Body);
    }

    private static async ValueTask Body(FileInfo file, CancellationToken token)
    {
        var bzipFile = Path.ChangeExtension(file.Name, "txt.bz2");

        await Cli.Wrap("bzip2")
            .WithArguments(args => args
                .Add(file.FullName)
                .Add("--compress")
                .Add("--force")
                .Add("--keep")
                .Add("--best"))
            .WithStandardOutputPipe(PipeTarget.ToDelegate(x => Console.Out.WriteLine(x)))
            .WithStandardErrorPipe(PipeTarget.ToDelegate(x => Console.Error.WriteLine(x)))
            .ExecuteAsync(token);

        File.Move(
            Path.Join(PathToOriginalFiles, bzipFile),
            Path.Join(PathToFolderContainingBZips, bzipFile));
        Console.WriteLine($"Done {bzipFile}");
    }

    [GeneratedRegex("""ca\.(?<t>\d)\.(?<v>\d)\^(?<k>\d+)\.txt""", RegexOptions.Compiled)]
    private static partial Regex FilenameRegex();
}