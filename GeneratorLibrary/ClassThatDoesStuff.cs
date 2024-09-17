using System.IO.Abstractions;
using System.Numerics;

namespace GeneratorLibrary;

public class ClassThatDoesStuff(
    IFileSystem fileSystem)
{
    public void DoStuff(string inputFile)
    {
        var result = Path.GetFileNameWithoutExtension(inputFile);
        var outputFile = $"{result}.cca";

        using var stream = fileSystem.File.Create(outputFile);
        Generator.TryWriteToBuffer(
            stream,
            [BigInteger.One],
            1);
    }
}