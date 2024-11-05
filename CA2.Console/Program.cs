using System.IO.Abstractions;

using CA2.Console;

using Cocona;

using CA2.Compression;
using CA2.Extractors;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

var builder = CoconaApp.CreateBuilder(args);
builder.Logging.AddDebug();

builder.Services.AddTransient<IFileSystem, FileSystem>();
builder.Services.AddTransient<ICompressor, Compressor>();
builder.Services.AddTransient<IExtractor, CsvExtractor>();
builder.Services.AddTransient<IExtractor, ActsExtractor>();

var app = builder.Build();

app.AddCommands<CompressCommand>();

await app.RunAsync();