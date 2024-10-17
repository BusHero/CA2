using System.IO.Abstractions;

using Cocona;

using GeneratorLibrary;
using GeneratorLibrary.Compression;

using Microsoft.Extensions.DependencyInjection;

var builder = CoconaApp.CreateBuilder(args);

builder.Services.AddTransient<IFileSystem, FileSystem>();
builder.Services.AddTransient<ICompressor, Compressor>();
builder.Services.AddTransient<CcaGenerator>();

var app = builder.Build();

app.AddCommand(async (
    string input,
    [Option("column", ['c'])]int[] columns,
    [FromService] CcaGenerator ccaGenerator) =>
{
    await ccaGenerator.GenerateCcaFile(input, columns);
});

await app.RunAsync();