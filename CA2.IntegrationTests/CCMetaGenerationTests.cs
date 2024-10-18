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

    private static async Task<string> GenerateRandomCsvFile()
    {
        var generator = new CsvFileGenerator(new DefaultRandomCsvGeneratorFactory());

        var filename = Path.Combine(
            "csvs",
            $"csv_{DateTime.Now:yyyy'-'MM'-'dd'T'hh'.'mm'.'ss'.'fff}");

        await using var stream = File.CreateText(filename);

        await generator.GenerateAsync(
            stream,
            100,
            [
                ["foo", "bar", "baz"],
                ["foo", "bar", "baz"],
                ["foo", "bar"],
            ]);

        return filename;
    }
}