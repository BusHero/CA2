namespace CA2.Tests.GeneratorTests;

using System.Numerics;

using GeneratorLibrary;

using Utils;

public sealed class GetNumberOfBitsForCombinationTests
{
    private readonly Generator _generator = new();

    [Property(Arbitrary = [typeof(CombinationsGenerator)])]
    public Property ResultIsBiggerThanZero(
        Combination combination)
    {
        var bytesPerCombination = _generator.GetNumberOfBitsForCombination(combination.Sizes);

        var property = 0 < bytesPerCombination;

        return property.ToProperty();
    }

    [Property(Arbitrary = [typeof(CombinationsGenerator)])]
    public Property ValueReturnedIsEnoughToStoreASingleNumber2(Combination combination)
    {
        var property = () =>
        {
            var bitsPerCombination = _generator.GetNumberOfBitsForCombination(combination.Sizes);

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
        var bitsPerCombination = _generator.GetNumberOfBitsForCombination([nbr.Get]);

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