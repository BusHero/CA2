using System.IO.Abstractions;

using GeneratorLibrary;

IFileSystem fileSystem = new FileSystem();
var foo = new ClassThatDoesStuff(fileSystem);
foo.DoStuff(args[0]);