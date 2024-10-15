using System.IO.Abstractions;

using CliWrap;

using CsvGenerator;

using Xunit.Abstractions;

namespace CA2.IntegrationTests;

public class CcMetaGenerationTests(
    ITestOutputHelper output)
{
    [Fact]
    public async Task RustGeneratorTest()
    {
        var filename = await GenerateRandomCsvFile();
        
        await Cli.Wrap(""".\bins\cca.exe""")
            .WithArguments(args => args
                .Add("--no-header")
                .Add(["-t", "2"])
                .Add(["-v", "3"])
                .Add(["-v", "3"])
                .Add(["-v", "2"])
                .Add(["-c", filename]))
            .WithStandardOutputPipe(PipeTarget.ToDelegate(output.WriteLine))
            .ExecuteAsync();
    }

    private async Task<string> GenerateRandomCsvFile()
    {
        var generator = new CsvGeneratorToFile(
            new FileSystem(),
            new DefaultRandomCsvGeneratorFactory());

        var filename = GenerateFilename("csv");
        await generator.GenerateAsync(
            "csvs",
            filename,
            100,
            [
                ["foo", "bar", "baz"],
                ["foo", "bar", "baz"],
                ["foo", "bar"],
            ]);

        return Path.Combine("csvs", $"{filename}.csv");
    }

    private static string GenerateFilename(string prefix)
        => $"{prefix}_{DateTime.Now:yyyy'-'MM'-'dd'T'hh'.'mm'.'ss'.'fff}";
}