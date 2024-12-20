﻿namespace MoveStuff;

public sealed class Program
{
    private const string Input = """C:\Users\Petru\projects\csharp\CA2\result-unarchive""";
    
    public static void Main()
    {
        var output = Path.Combine(Input, "ca2");
        var directory = new DirectoryInfo(Input);
        directory.Create();
        
        var files = directory
            .EnumerateFiles("*.ccmeta")
            .Select(f => f.FullName);
        
        Parallel.ForEach(files, (file, _) =>
        {
            File.Move(file, Path.Combine(output, Path.GetFileName(file)));
            Console.WriteLine(file);
        });
    }
}