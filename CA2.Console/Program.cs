using System.IO.Abstractions;

using CA2.Console;

using Cocona;

using CA2.Compression;

using Microsoft.Extensions.DependencyInjection;

var builder = CoconaApp.CreateBuilder(args);

builder.Services.AddTransient<IFileSystem, FileSystem>();
builder.Services.AddTransient<ICompressor, Compressor>();

var app = builder.Build();

app.AddCommands<CompressCommand>();

await app.RunAsync();