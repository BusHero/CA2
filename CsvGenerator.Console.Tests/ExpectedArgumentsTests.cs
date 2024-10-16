namespace CsvGenerator.Console.Tests;

using System.Threading.Tasks;

using Xunit.Abstractions;

public class ExpectedArgumentsTests
{
    public ExpectedArgumentsTests(ITestOutputHelper outputHelper)
    {
        System.Console.SetOut(new OutputHelperTextWriter(outputHelper));
        System.Console.SetError(new OutputHelperTextWriter(outputHelper));
    }

    [Theory, MemberData(nameof(Args))]
    public async Task ProgramCanHandleExpectedNumberCombinations(string[] args)
    {
        await Program.Main(args);

        Environment
            .ExitCode
            .Should()
            .Be(0);
    }

    public static TheoryData<string[]> Args => [
        ["--rows", "100", "--columns", "one,two,three", "--filename", "foo"],
        ["--rows", "100", "--columns", "one,two,three", "--columns", "five,six,seven", "--filename", "foo"],
        ["--rows", "100", "--columns", "one,two,three", "--columns", "five,six,seven", "--filename", "foo", "--destination", "bar"],
    ];
}
