using System.IO.Abstractions;

using Cocona;

using CsvGenerator;
using CsvGenerator.Console;

using Microsoft.Extensions.DependencyInjection;

var builder = CoconaApp.CreateBuilder(args);

builder.Services.AddTransient<ICsvGenerator, CsvGenerator.CsvGenerator>();
builder.Services.AddTransient<IRandomCsvGeneratorFactory, DefaultRandomCsvGeneratorFactory>();
builder.Services.AddTransient<IFileSystem, FileSystem>();

var app = builder.Build();

app.AddCommands<CsvGeneratorCommand>();

await app.RunAsync();