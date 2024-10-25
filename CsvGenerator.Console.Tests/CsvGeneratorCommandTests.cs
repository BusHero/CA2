namespace CsvGenerator.Console.Tests;

using System.IO;
using System.IO.Abstractions.TestingHelpers;
using System.Threading.Tasks;

using FluentAssertions.Execution;

using NSubstitute;

public class CsvGeneratorCommandTests
{
    [Theory, AutoData]
    public async Task CommandWorksAsync(
        int rows,
        string[][] columns,
        string[][] expectedCsv,
        string filename,
        string destination)
    {
        var realColumns = columns.Select(x => string.Join(",", x)).ToArray();
        var csvFileGenerator = new MockCsvGenerator(expectedCsv);
        var factory = Substitute.For<ICsvGeneratorFactory>();

        factory.Create().Returns(csvFileGenerator);

        var fileSystem = new MockFileSystem();

        var command = new CsvGeneratorCommand(factory, fileSystem);

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
            realCsv
                .Split(Environment.NewLine)
                .Select(x => x.Split(','))
                .Should()
                .BeEquivalentTo(expectedCsv);
        }
    }
}
