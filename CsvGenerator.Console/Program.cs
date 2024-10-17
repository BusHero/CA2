using System.IO.Abstractions;

using Cocona;

using CsvGenerator;
using CsvGenerator.Console.Tests;

using Microsoft.Extensions.DependencyInjection;

public sealed class Program
{
    public static async Task Main(string[] args)
    {
        var builder = CoconaApp.CreateBuilder(args);

        builder.Services.AddTransient<CsvFileGenerator>();
        builder.Services.AddTransient<IRandomCsvGeneratorFactory, DefaultRandomCsvGeneratorFactory>();
        builder.Services.AddTransient<IFileSystem, FileSystem>();

        var app = builder.Build();

        app.AddCommands<CsvGeneratorCommand>();

        await app.RunAsync();
    }
}