using CA2.Extractors;

var directory = new DirectoryInfo("""C:\Users\Petru\projects\csharp\CA2\result-unarchive""");

var files = directory.EnumerateFiles("*.txt");

await Parallel.ForEachAsync(files, async (file, token) =>
{
    var extractor = new ActsExtractor();
    using var stream = file.OpenText();

    var result = await extractor.ExtractAsync(stream);
    var lines = result.Select(x => string.Join(',', x));

    var outputFilename = Path.Combine(
        directory.FullName,
        Path.ChangeExtension(file.Name, "csv"));
    await File.WriteAllLinesAsync(outputFilename, lines, token);

    Console.WriteLine($@"✓ {outputFilename}");
});