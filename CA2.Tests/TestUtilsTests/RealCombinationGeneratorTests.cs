namespace CA2.Tests.TestUtilsTests;

using CA2.Tests.Utils;

public sealed class RealCombinationGeneratorTests
{
    [Property(Arbitrary = [typeof(CombinationsGenerator)])]
    public Property SizesLengthShouldBeBiggerThanZero(RealCombination realCombination)
    {
        return (0 < realCombination.Sizes.Length)
            .And(realCombination.Sizes.All(size => 2 <= size));
    }

    [Property(Arbitrary = [typeof(CombinationsGenerator)])]
    public Property ItemsIsBiggerThanZero(RealCombination realCombination)
    {
        var allItemsAreBetweenZeroAndSizeMinus1 = realCombination.Items
            .All(x => x
                .Select((y, i) => 0 <= y && y < realCombination.Sizes[i])
                .All(x => x));

        return (0 < realCombination.Items.Length)
            .And(realCombination.Items.All(x => x.Length == realCombination.Sizes.Length))
            .And(allItemsAreBetweenZeroAndSizeMinus1)
            ;
    }

    [Property]
    public Property LengthOfSizeDependsOnSize()
    {
        var arb =
            Gen.Sized(size => CombinationsGenerator
                .RealCombination()
                .Generator
                .Resize(size)
                .Select(combination => (combination, size)))
            .ToArbitrary();
            
        return Prop.ForAll(
            arb,
            x => x.combination.Sizes.Length == x.size);
    }

    [Property]
    public Property Foo()
    {
        var arb =
            Gen.Sized(size => Arb.Default
                .Array<int>()
                .Generator
                .Resize(size)
                .Select(arr => (arr, size)))
            .ToArbitrary();

        return Prop.ForAll(arb, t => t.arr.Length == t.size);
    }
}
