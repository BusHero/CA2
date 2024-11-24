using System.Text.RegularExpressions;

namespace GenerateBigCsv;

public partial class Program
{
    private const string OriginalFileFolder = """C:\Users\Petru\projects\csharp\CA2\result-unarchive""";
    private const string ZipFileFolder = """C:\Users\Petru\projects\csharp\CA2\result""";
    private const string FilesThatMyProgramGenerated = """C:\Users\Petru\projects\csharp\CA2\result-ca2""";
    private const string OriginalProgram = """C:\Users\Petru\projects\csharp\CA2\result-origin""";

    public static async Task Main()
    {
        var directory = new DirectoryInfo(OriginalFileFolder);

        await using var writer = File.CreateText("""C:\Users\Petru\projects\csharp\CA2\result.csv""");

        await writer.WriteLineAsync(
            "Filename,T,V,K,rows,original-file-length,zip,cca-new,ccmeta-new,cca-original,ccmeta-original");

        foreach (var file in directory.EnumerateFiles("*.txt"))
        {
            var rows = await GetRows(file);
            var zipFile = new FileInfo(GetZipFilePath(ZipFileFolder, file.FullName, ".txt.zip"));
            var myCcaFile = new FileInfo(GetZipFilePath(FilesThatMyProgramGenerated, file.FullName, ".cca"));
            var myMetaFile = new FileInfo(GetZipFilePath(FilesThatMyProgramGenerated, file.FullName, ".ccmeta"));
            var originalCcaFile = new FileInfo(GetZipFilePath(OriginalProgram, file.FullName, ".cca"));
            var originalMetaFile = new FileInfo(GetZipFilePath(OriginalProgram, file.FullName, ".ccmeta"));

            var match = Filename().Match(file.Name);
            var t = match.Groups["t"].Value;
            var k = match.Groups["k"].Value;
            var v = match.Groups["v"].Value;
            await writer.WriteLineAsync(
                $"{file.Name},{t},{v},{k},{rows},{file.Length},{zipFile.Length},{myCcaFile.Length},{myMetaFile.Length},{originalCcaFile.Length},{originalMetaFile.Length}");
            await writer.FlushAsync();
        }
    }

    private static async Task<int> GetRows(FileInfo file)
    {
        await using var stream = file.OpenRead();
        using var reader = new StreamReader(stream);

        var line = await reader.ReadLineAsync();

        return int.Parse(line!);
    }

    private static string GetZipFilePath(string directory, string originalFileName, string extension)
        => Path.Combine(
            directory,
            Path.ChangeExtension(originalFileName, extension));

    [GeneratedRegex("""ca\.(?<t>\d)\.(?<v>\d)\^(?<k>\d+)\.txt""", RegexOptions.Compiled)]
    private static partial Regex Filename();
}