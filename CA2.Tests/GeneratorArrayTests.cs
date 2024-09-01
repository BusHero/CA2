using System.Numerics;
using GeneratorLibrary;

namespace CA2.Tests;

public sealed class GeneratorArrayTests
{
    [Property(Arbitrary = [typeof(Generators)])]
    public Property ResultIsBiggerThanZero(
        Combination combination)
    {
        var bytesPerCombination = Generator.GetNumberOfBitsFoCombination(combination.Sizes);

        var property = 0 < bytesPerCombination;

        return property.ToProperty();
    }

    [Property(Arbitrary = [typeof(Generators)])]
    public Property ValueReturnedIsEnoughToStoreASingleNumber2(Combination combination)
    {
        var property = () =>
        {
            var bitsPerCombination = Generator.GetNumberOfBitsFoCombination(combination.Sizes);

            var maxNumber = CalculateMaximumNumber(combination.Sizes);

            var bitsToStoreTheBiggestNumber = maxNumber.GetBitLength();

            return bitsToStoreTheBiggestNumber == bitsPerCombination;
        };

        return property
            .When(combination is { Sizes.Length: > 2 });
    }

    [Property(Arbitrary = [typeof(Generators)])]
    public Property ValueReturnedIsEnoughToStoreASingleNumber(
        PositiveInt nbr)
    {
        var bitsPerCombination = Generator.GetNumberOfBitsFoCombination([nbr.Get]);

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