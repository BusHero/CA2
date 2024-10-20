namespace CsvGenerator.Console.Tests;

using System.IO;
using System.IO.Abstractions.TestingHelpers;
using System.Threading.Tasks;

using FluentAssertions.Execution;

public class CsvGeneratorCommandTests
{
    [Theory, AutoData]
    public async Task CommandWorksAsync(
        int rows,
        string[][] columns,
        string expectedCsv,
        string filename,
        string destination)
    {
        var realColumns = columns.Select(x => string.Join(",", x)).ToArray();
        var csvFileGenerator = new MockCsvGenerator(expectedCsv);
        var fileSystem = new MockFileSystem();

        var command = new CsvGeneratorCommand(csvFileGenerator, fileSystem);

        await command.Command(
            rows: rows,
            columns: realColumns,
            filename: filename,
            destination: destination);

        using (new AssertionScope())
        {
            csvFileGenerator
                .RowsCount
                .Should()
                .Be(rows);

            csvFileGenerator
                .Columns
                .Should()
                .BeEquivalentTo(columns);

            var realCsv = await fileSystem.File.ReadAllTextAsync(Path.Combine(destination, $"{filename}.csv"));

            realCsv.Should().BeEquivalentTo(expectedCsv);
        }
    }
}

public class MockCsvGenerator(string csv) : ICsvGenerator
{
    public string[][]? Columns { get; private set; }

    public int? RowsCount { get; private set; }

    public async Task GenerateAsync(TextWriter writer, int rowsCount, string[][] columns)
    {
        RowsCount = rowsCount;
        Columns = columns;

        await writer.WriteAsync(csv);
    }
}
