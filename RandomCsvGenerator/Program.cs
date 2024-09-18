using CommandLine;

using GeneratorLibrary;

var foo = Parser.Default.ParseArguments<Options>(args)
    .WithParsed(o =>
    {
        DoStuff(o.Filename, o.Rows, o.Columns.ToArray());
    });


void DoStuff(string? filename, int rowsCount, int[] columns)
{
    var outputFile = $"{filename}.csv";
    var generator = new RandomCsvGenerator();

    var csv = generator
        .WithColumns(columns)
        .WithRowsCount(rowsCount)
        .Generate();

    Console.WriteLine($"Write to {outputFile}");

    var rows = csv.Select(x => string.Join(',', x)).ToList();
    foreach(var row in rows)
    {
        Console.WriteLine(row);
    }

    File.WriteAllLines(outputFile, rows);
}


public sealed record Options
{
    [Option("filename", HelpText = "Set name of the output file")]
    public string? Filename { get; set; }

    [Option("rows", HelpText = "Number of rows")]
    public int Rows { get; set; }

    [Option]
    public IEnumerable<int> Columns { get; set; } = default!;
}
