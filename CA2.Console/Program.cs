using System.IO.Abstractions;

using Cocona;

using GeneratorLibrary;
using GeneratorLibrary.Compression;

using Microsoft.Extensions.DependencyInjection;

namespace CA2.Console;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = CoconaApp.CreateBuilder(args);

        builder.Services.AddTransient<IFileSystem, FileSystem>();
        builder.Services.AddTransient<ICompressor, Compressor>();
        builder.Services.AddTransient<CcaGenerator>();

        var app = builder.Build();

        app.AddCommand(async (
            string filename,
            int[] columns,
            [FromService] CcaGenerator ccaGenerator) =>
        {
            await ccaGenerator.GenerateCcaFile(filename, columns);
            System.Console.WriteLine(filename);
        });

        await app.RunAsync();
    }
}