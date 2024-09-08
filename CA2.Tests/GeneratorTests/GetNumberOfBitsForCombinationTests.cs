using System.Numerics;
using GeneratorLibrary;

namespace CA2.Tests.GeneratorTests;

public sealed class GetNumberOfBitsForCombinationTests
{
    [Property(Arbitrary = [typeof(CombinationsGenerator)])]
    public Property ResultIsBiggerThanZero(
        Combination combination)
    {
        var bytesPerCombination = Generator.GetNumberOfBitsForCombination(combination.Sizes);

        var property = 0 < bytesPerCombination;

        return property.ToProperty();
    }

    [Property(Arbitrary = [typeof(CombinationsGenerator)])]
    public Property ValueReturnedIsEnoughToStoreASingleNumber2(Combination combination)
    {
        var property = () =>
        {
            var bitsPerCombination = Generator.GetNumberOfBitsForCombination(combination.Sizes);

            var maxNumber = CalculateMaximumNumber(combination.Sizes);

            var bitsToStoreTheBiggestNumber = maxNumber.GetBitLength();

            return bitsToStoreTheBiggestNumber == bitsPerCombination;
        };

        return property
            .When(combination is { Sizes.Length: > 2 });
    }

    [Property(Arbitrary = [typeof(CombinationsGenerator)])]
    public Property ValueReturnedIsEnoughToStoreASingleNumber(
        PositiveInt nbr)
    {
        var bitsPerCombination = Generator.GetNumberOfBitsForCombination([nbr.Get]);

        var lengthOfBitString = nbr.Get.ToString("b").Length;

        var property = lengthOfBitString == bitsPerCombination;

        return property
            .When(2 <= nbr.Get)
            .Label($"For {nbr.Get} ToString({lengthOfBitString}) == Generator({bitsPerCombination})");
    }

    private static BigInteger CalculateMaximumNumber(IEnumerable<int> sizes) =>
        sizes
            .Aggregate(BigInteger.One, (r, x) => r * x);
}