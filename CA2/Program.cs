using System.IO.Abstractions;

using GeneratorLibrary;

IFileSystem fileSystem = new FileSystem();
var foo = new CcaGenerator(fileSystem, default!);
foo.GenerateCcaFile(args[0]);