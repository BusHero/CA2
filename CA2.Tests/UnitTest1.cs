using FluentAssertions;

namespace CA2.Tests;

public class UnitTest1
{
    public static TheoryData<int[], int> Data => new()
    {
        { [0, 0], 0 },
        { [0, 1], 1 },
        { [1, 0], 2 },
        { [1, 1], 3 },
        { [0, 0, 0], 0 },
        { [0, 0, 1], 1 },
        { [0, 1, 0], 2 },
        { [0, 1, 1], 3 },
        { [1, 0, 0], 4 },
        { [1, 0, 1], 5 },
        { [1, 1, 0], 6 },
        { [1, 1, 1], 7 },
    };

    [Theory]
    [MemberData(nameof(Data))]
    public void GenerateNumber(int[] values, int expectedResult)
    {
        var generator = new Generator();

        var number = generator.Generate(values);

        number.Should().Be(expectedResult);
    }
}

public sealed class Generator
{
    public long Generate(int[] values)
    {
        var result = 0;
        
        for (var i = 0; i < values.Length - 1; i++)
        {
            result *= 2;
            result += values[i] * 2;
        }

        return result + values[^1];
    }
}
/*
 * Given I have the following
 *
 */