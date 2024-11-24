using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

using CsvHelper;
using CsvHelper.Configuration.Attributes;

namespace GenerateBigCsv;

public partial class Program
{
    private const string OriginalFileFolder = """C:\Users\Petru\projects\csharp\CA2\result-unarchive""";
    private const string ZipFileFolder = """C:\Users\Petru\projects\csharp\CA2\result-zip""";
    private const string FilesThatMyProgramGenerated = """C:\Users\Petru\projects\csharp\CA2\result-ca2""";
    private const string OriginalProgram = """C:\Users\Petru\projects\csharp\CA2\result-origin""";
    private const string BZip2 = """C:\Users\Petru\projects\csharp\CA2\result-bzip2""";

    public static async Task Main()
    {
        var directory = new DirectoryInfo(OriginalFileFolder);

        await using var writer = File.CreateText("""C:\Users\Petru\projects\csharp\CA2\result.csv""");
        await using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);

        csv.WriteHeader<Record>();
        await csv.NextRecordAsync();

        foreach (var file in directory.EnumerateFiles("*.txt"))
        {
            await WriteRecord(file, csv);
        }
    }

    private static async Task WriteRecord(FileInfo file, CsvWriter csv)
    {
        var rows = await GetRows(file);
        var zipFile = new FileInfo(GetFilePath(ZipFileFolder, file.Name, ".zip"));
        var myCcaFile = new FileInfo(GetFilePath(FilesThatMyProgramGenerated, file.Name, ".cca"));
        var myMetaFile = new FileInfo(GetFilePath(FilesThatMyProgramGenerated, file.Name, ".ccmeta"));
        var originalCcaFile = new FileInfo(GetFilePath(OriginalProgram, file.Name, ".cca"));
        var originalMetaFile = new FileInfo(GetFilePath(OriginalProgram, file.Name, ".ccmeta"));
        var bzip = new FileInfo(GetFilePath(BZip2, file.Name, ".txt.bz2"));

        var match = FilenameRegex().Match(file.Name);

        csv.WriteRecord(new Record
        {
            Filename = file.Name,
            Strength = match.Groups["t"].Value,
            ValuesPerColumn = match.Groups["v"].Value,
            NumberOfColumns = match.Groups["k"].Value,
            Rows = rows,
            OriginalFileLength = file.Length,
            ZipFileLength = zipFile.Length,
            CcaFileLength = myCcaFile.Length,
            MetafileLength = myMetaFile.Length,
            OriginalCcaFileLength = originalCcaFile.Length,
            OriginalMetafileLength = originalMetaFile.Length,
            BZip = bzip.Length,
        });
        
        await csv.NextRecordAsync();
        await csv.FlushAsync();
    }

    private readonly static byte[] Buffer = new byte[10];

    private static async ValueTask<string> GetRows(FileInfo file)
    {
        await using var stream = file.OpenRead();
        var memory = new Memory<byte>(Buffer);
        _ = await stream.ReadAsync(memory);

        var index = memory.Span.IndexOf((byte)'\n');

        return Encoding.ASCII.GetString(memory[..index].Span);
    }

    private static string GetFilePath(string directory, string originalFileName, string extension)
        => Path.Combine(
            directory,
            Path.ChangeExtension(originalFileName, extension));

    [GeneratedRegex("""ca\.(?<t>\d)\.(?<v>\d)\^(?<k>\d+)\.txt""", RegexOptions.Compiled)]
    private static partial Regex FilenameRegex();

    [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
    public record struct Record
    {
        [Name("Filename"), Index(0)] public required string Filename { get; init; }

        [Name("T"), Index(1)] public required string Strength { get; init; }

        [Name("V"), Index(2)] public required string ValuesPerColumn { get; init; }

        [Name("K"), Index(3)] public required string NumberOfColumns { get; init; }

        [Name("Rows"), Index(4)] public required string? Rows { get; init; }

        [Name("original-file-length"), Index(5)]
        public required long OriginalFileLength { get; init; }

        [Name("zip"), Index(6)] public required long ZipFileLength { get; init; }

        [Name("cca-new"), Index(7)] public required long CcaFileLength { get; init; }

        [Name("ccmeta-new"), Index(8)] public required long MetafileLength { get; init; }

        [Name("cca-original"), Index(9)] public required long OriginalCcaFileLength { get; init; }

        [Name("ccmeta-original"), Index(10)] public required long OriginalMetafileLength { get; init; }

        [Name("bzip2"), Index(10)] public required long BZip { get; init; }
    }
}