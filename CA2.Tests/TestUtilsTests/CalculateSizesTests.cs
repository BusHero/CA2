namespace CA2.Tests.TestUtilsTests;

using System.Numerics;

using TestUtils;

public sealed class CalculateSizesTests
{
    [Property]
    public Property LastItemIsEqualToOne(NonEmptyArray<PositiveInt> sizes)
    {
        var actualSizes = sizes.Item
            .Select(x => x.Item)
            .ToArray();

        var generatedSequence = actualSizes.CalculateSizes();

        var property = () => generatedSequence[^1] == 1;

        return property
            .Label("Last item is 1");
    }

    [Property]
    public Property FirstItemIsEqualToTheLastOne()
    {
        var arb = Arb.Default.PositiveInt()
            .Generator
            .Select(x => x.Item)
            .ArrayOf(2)
            .ToArbitrary();

        return Prop.ForAll(arb, sizes =>
        {
            var generatedSequence = sizes.CalculateSizes();

            var property = generatedSequence[0] == sizes[^1];

            return property
                .Label("Last item is equal to the last one");
        });
    }

    [Property]
    public Property GeneratedArrayIsArrangedDescending(NonEmptyArray<PositiveInt> sizes)
    {
        var actualSizes = sizes.Item
            .Select(x => x.Item)
            .ToArray();

        var generatedSequence = actualSizes.CalculateSizes();

        return generatedSequence
            .OrderDescending()
            .SequenceEqual(generatedSequence)
            .Label($"The generated array should be sorted [{string.Join(", ", generatedSequence)}]");
    }

    [Property]
    public Property SizeOfGeneratedArrayIsEqualToTheOriginalSize(NonEmptyArray<PositiveInt> sizes)
    {
        var actualSizes = sizes.Item
            .Select(x => x.Item)
            .ToArray();

        var generatedSequence = actualSizes.CalculateSizes();

        var property = generatedSequence.Length == sizes.Item.Length;

        return property
            .Label($"{generatedSequence.Length} == {sizes.Item.Length}");
    }

    [Property]
    public Property FirstItemIsTheProductOfPreviousNumbers(NonEmptyArray<PositiveInt> sizes)
    {
        var actualSizes = sizes.Item
            .Select(x => x.Item)
            .ToArray();

        var generatedSequence = actualSizes.CalculateSizes();

        var property = () => generatedSequence[0] == sizes.Item.Skip(1)
            .Select(x => x.Item)
            .Select(x => (BigInteger)x)
            .Aggregate(BigInteger.One, (fst, snd) => fst * snd);

        return property
            .When(2 <= sizes.Item.Length);
    }

    [Property(MaxTest = 10_000)]
    public Property ItemIsEqualToThePreviousGeneratedValueAndOriginalSize(NonEmptyArray<PositiveInt> sizes)
    {
        var actualSizes = sizes.Item
            .Select(x => x.Item)
            .ToArray();

        var generatedSequence = actualSizes.CalculateSizes();

        var property = () => generatedSequence
            .SkipLast(1)
            .Select((x, i) => (x, index: i + 1))
            .All(t => t.x == generatedSequence[t.index] * sizes.Item[t.index].Item);

        return property
            .When(2 <= sizes.Item.Length);
    }

    [Property]
    public Property ItemIsEqualToThePreviousGeneratedValueAndOriginalSizeSizeIs3()
    {
        var arb = Arb.Default.PositiveInt()
            .Generator
            .Select(x => x.Item)
            .ArrayOf(3)
            .ToArbitrary();

        return Prop.ForAll(arb, sizes =>
        {
            var generatedSequence = sizes.CalculateSizes();

            var property = () => generatedSequence
                .SkipLast(1)
                .Select((x, i) => (x, index: i + 1))
                .All(t => t.x == generatedSequence[t.index] * sizes[t.index]);

            return property
                .ToProperty();
        });
    }
}