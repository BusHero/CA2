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
        if (values[0] == 1 && values[1] == 1)
        {
            return 3;
        }

        if (values[1] == 1)
        {
            return 1;
        }

        if (values[0] == 1)
        {
            return 2;
        }

        return 0;
    }
}
/*
 * Given I have the following
 *
 */