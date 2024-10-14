using System.CommandLine;

using CsvGenerator.Console;

var rootCommand = Root.GetRootCommand();

return await rootCommand.InvokeAsync(args);