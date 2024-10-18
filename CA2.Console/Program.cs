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

app.AddCommands<CompressCommand>();

await app.RunAsync();
