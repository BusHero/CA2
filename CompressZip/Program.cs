using System.IO.Compression;
using System.Text.RegularExpressions;

namespace CompressZip;

internal partial class Program
{
    private const string PathToOriginalFiles = """C:\Users\Petru\projects\csharp\CA2\result-unarchive""";
    private const string PathToFolderContainingZips = """C:\Users\Petru\projects\csharp\CA2\result-zip""";

    public static async Task Main()
    {
        Directory.CreateDirectory(PathToFolderContainingZips);
        var directory = new DirectoryInfo(PathToOriginalFiles);

        var files = directory
            .EnumerateFiles("*.txt");

        await Parallel.ForEachAsync(files, Body);
    }

    private static async ValueTask Body(FileInfo file, CancellationToken _)
    {
        var zipFile = Path.Combine(
            PathToFolderContainingZips,
            Path.ChangeExtension(file.Name, "zip"));

        await using var stream = File.Create(zipFile);
        
        using var archive = new ZipArchive(stream, ZipArchiveMode.Create);
        archive.CreateEntryFromFile(
            file.FullName,
            file.Name,
            CompressionLevel.SmallestSize);
        
        Console.WriteLine($"Done - {zipFile}");
    }

    [GeneratedRegex("""ca\.(?<t>\d)\.(?<v>\d)\^(?<k>\d+)\.txt""", RegexOptions.Compiled)]
    private static partial Regex FilenameRegex();
}