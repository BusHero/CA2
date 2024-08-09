
using FluentAssertions;

namespace CA2.Tests;

public class UnitTest1
{
    [Fact]
    public void GenerateNumber()
    {
        var generator = new Generator();

        int[] values = [0, 0];
        
        var number = generator.Generate(values);

        number.Should().Be(0);
    }
    
    [Fact]
    public void GenerateNumber2()
    {
        var generator = new Generator();

        int[] values = [0, 1];
        
        var number = generator.Generate(values);

        number.Should().Be(1);
    }
}

public sealed class Generator
{
    public long Generate(int[] values)
    {
        if (values[1] == 1)
        {
            return 1;
        }
        
        return 0;
    }
}
/*
 * Given I have the following 
 * 
 */