using Cocona;

using GeneratorLibrary;

public sealed class CompressCommand(CcaGenerator ccaGenerator)
{
    public async Task Command(
        string input,
        [Option("column", ['c'])] int[] columns)
    {
        await ccaGenerator.GenerateCcaFile(input, columns);
    }
}