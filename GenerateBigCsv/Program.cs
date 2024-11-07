using System.Text.RegularExpressions;

namespace GenerateBigCsv;

public partial class Program
{
    private const string OriginalFileFolder = """C:\Users\Petru\projects\csharp\CA2\result-unarchive""";
    private const string ZipFileFolder = """C:\Users\Petru\projects\csharp\CA2\result""";
    private const string FilesThatMyProgramGenerated = """C:\Users\Petru\projects\csharp\CA2\result-ca2""";
    private const string OriginalProgram = """C:\Users\Petru\projects\csharp\CA2\result-original""";

    public static async Task Main()
    {
        var directory = new DirectoryInfo(OriginalFileFolder);
        var files = directory.GetFiles("*.txt");

        await using var writer = File.CreateText("""C:\Users\Petru\projects\csharp\CA2\result.csv""");
        
        await writer.WriteLineAsync("Filename,T,V,K,original-file-length,zip,cca-new,ccmeta-new,cca-old,ccmeta-old");
        
        foreach (var file in files)
        {
            var zipFile = new FileInfo(GetZipFilePath(ZipFileFolder, file.FullName, ".txt.zip"));
            var myCcaFile = new FileInfo(GetZipFilePath(FilesThatMyProgramGenerated, file.FullName, ".cca"));
            var myMetaFile = new FileInfo(GetZipFilePath(FilesThatMyProgramGenerated, file.FullName, ".ccmeta"));
            var originalCcaFile = new FileInfo(GetZipFilePath(OriginalProgram, file.FullName, ".cca"));
            var originalMetaFile = new FileInfo(GetZipFilePath(OriginalProgram, file.FullName, ".ccmeta"));

            var match = Filename().Match(file.Name);
            var t = byte.Parse(match.Groups["t"].Value);
            var k = int.Parse(match.Groups["k"].Value);
            var v = int.Parse(match.Groups["v"].Value);
            await writer.WriteLineAsync(
                $"{file.Name},{t},{v},{k},{file.Length},{zipFile.Length},{myCcaFile.Length},{myMetaFile.Length},{originalCcaFile.Length},{originalMetaFile.Length}");
            await writer.FlushAsync();
        }
    }

    private static string GetZipFilePath(string directory, string originalFileName, string extension)
    {
        var fileWithoutExtension = Path.GetFileNameWithoutExtension(originalFileName);

        return Path.Combine(directory, $"{fileWithoutExtension}{extension}");
    }

    [GeneratedRegex("""ca\.(?<t>\d)\.(?<v>\d)\^(?<k>\d+)\.txt""")]
    private static partial Regex Filename();
}