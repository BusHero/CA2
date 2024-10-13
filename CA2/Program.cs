using System.IO.Abstractions;

using GeneratorLibrary;
using GeneratorLibrary.Compression;

IFileSystem fileSystem = new FileSystem();
var compressor = new Compressor();
var foo = new CcaGenerator(fileSystem, compressor);
await foo.GenerateCcaFile(args[0], []);