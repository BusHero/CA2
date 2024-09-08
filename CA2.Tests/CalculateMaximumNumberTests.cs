using System.Numerics;

namespace CA2.Tests;

public sealed class CalculateMaximumNumberTests
{
    public static TheoryData<int[], BigInteger> Sizes => new()
    {
        { [2], 2 },
        { [3], 3 },
        { [10], 10 },
        { [10, 10], 100 },
        { [10, 10, 10], 1000 },
        { [2, 2, 2], 8 },
        { [2, 7, 3, 2, 2, 2, 2], 672 }
    };
    
    [Theory, MemberData(nameof(Sizes))]
    public void MaximumNumberIsCalculatedCorrectly(int[] sizes, BigInteger expectedResult)
    {
        var result = TestUtils.CalculateMaximumNumber(sizes);

        result.Should().Be(expectedResult);
    }
}