using System.IO.Abstractions;

using GeneratorLibrary;

IFileSystem fileSystem = new FileSystem();
var foo = new CcaGenerator(fileSystem, default!);
await foo.GenerateCcaFile(args[0]);