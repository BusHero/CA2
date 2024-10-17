using System.IO.Abstractions;

using Cocona;

using CsvGenerator;

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

        app.AddCommand(async (
            CsvFileGenerator service,
            IFileSystem fileSystem,
            int rows,
            [Option] string[] columns,
            string filename,
            string? destination) =>
        {
            var realColumns = columns.Select(x => x.Split(',').ToArray()).ToArray();

            await service.GenerateAsync(
                destination ?? fileSystem.Directory.GetCurrentDirectory(), 
                filename, 
                rows, 
                realColumns);
        });

        await app.RunAsync();
    }
}