using System.IO.Abstractions;

using Cocona;

using Microsoft.Extensions.DependencyInjection;

namespace CsvGenerator.Console;

public sealed class Program
{
    public static async Task Main(string[] args)
    {
        var builder = CoconaApp.CreateBuilder(args);

        builder.Services.AddTransient<ICsvFileGenerator, CsvFileGenerator>();
        builder.Services.AddTransient<IRandomCsvGeneratorFactory, DefaultRandomCsvGeneratorFactory>();
        builder.Services.AddTransient<IFileSystem, FileSystem>();

        var app = builder.Build();

        app.AddCommand(CsvGeneratorCommand.Command);

        await app.RunAsync();
    }
}