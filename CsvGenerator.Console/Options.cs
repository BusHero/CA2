using System.CommandLine;

namespace CsvGenerator.Console;

internal static class Options
{
    public static Option<string> Destination { get; }
    
    public static Option<string> Filename { get; }

    public static Option<int> Rows { get; }

    public static Option<string[][]> Columns { get; }

    static Options()
    {
        Destination = new Option<string>(
            "--destination",
            description: "Destination file path",
            getDefaultValue: Directory.GetCurrentDirectory)
        {
            IsRequired = false,
        };
        
        Rows = new Option<int>(
            "--rows",
            description: "Number of rows to generate")
        {
            IsRequired = true,
        };

        Filename = new Option<string>(
            "--filename",
            description: "Name of the generated CSV file")
        {
            IsRequired = true,
        };

        Columns = new Option<string[][]>(
            "--columns",
            description: "Columns values to generate",
            parseArgument: result => result.Tokens.Select(x => x.Value.Split(',')).ToArray())
        {
            IsRequired = true,
            AllowMultipleArgumentsPerToken = true,
        };
    }
}