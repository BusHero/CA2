using System.CommandLine;

namespace RandomCsvGenerator.Console;

internal static class Root
{
    public static RootCommand GetRootCommand()
    {
        var rootCommand = new RootCommand("App to generate random CSV files");
        rootCommand.AddOption(Options.Rows);
        rootCommand.AddOption(Options.Columns);
        rootCommand.AddOption(Options.Filename);
        rootCommand.AddOption(Options.Destination);

        rootCommand.SetHandler(
            GeneratorHandler.HandleAsync,
            Options.Destination,
            Options.Filename,
            Options.Rows,
            Options.Columns);
        
        return rootCommand;
    }
}