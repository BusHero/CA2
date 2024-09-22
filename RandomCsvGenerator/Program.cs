using System.CommandLine;

using RandomCsvGenerator;

var rootCommand = Root.GetRootCommand();

return await rootCommand.InvokeAsync(args);