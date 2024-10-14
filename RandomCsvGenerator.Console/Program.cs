using System.CommandLine;

using RandomCsvGenerator;
using RandomCsvGenerator.Console;

var rootCommand = Root.GetRootCommand();

return await rootCommand.InvokeAsync(args);