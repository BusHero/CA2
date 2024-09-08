namespace CA2.Tests;

public sealed class CombinationsGeneratorTests
{
    [Property(Arbitrary = [typeof(CombinationsGenerator),])]
    public Property FirstItemIsSmallerThanSecondItem(
        (int, int) x)
    {
        return (x.Item1 < x.Item2).Label("First item is smaller than Second item")
            .And(0 <= x.Item1).Label("First item is bigger than 0")
            .And(2 <= x.Item2).Label("Second item is at least 2");
    }

    [Property(Arbitrary = [typeof(CombinationsGenerator),])]
    public Property LetsTryThis(
        (int, int)[] items)
    {
        return items.All(x => x.Item1 < x.Item2).Label("First item is smaller than Second item")
            .And(items.All(x => 0 <= x.Item1)).Label("First item is bigger than 0")
            .And(items.All(x => 2 <= x.Item2)).Label("Second item is at least 2");
    }

    [Property]
    public Property SizesAreDifferent()
    {
        var gen = CombinationsGenerator
            .Generator
            .ArrayOf(1000)
            .ToArbitrary();

        return Prop.ForAll(gen, items => items
            .Select(x => x.Item1)
            .All(x => x == items[0].Item1) == false);
    }

    [Property]
    public Property SecondItemIsEqualToSize(PositiveInt size)
    {
        var expectedSize = size.Item < 2
            ? 2
            : size.Item;

        var gen = CombinationsGenerator
            .Generator
            .ArrayOf(10)
            .Resize(size.Item)
            .ToArbitrary();

        return Prop
            .ForAll(gen, items => items
                .Select(x => x.Item2)
                .All(x => x == expectedSize));
    }

    [Theory]
    [InlineData(5)]
    [InlineData(6)]
    [InlineData(7)]
    [InlineData(8)]
    public void FirstItemIsEvenlySpread(int size)
    {
        const int arraySize = 10_000;

        var gen = CombinationsGenerator
            .Generator
            .ArrayOf(arraySize)
            .Resize(size)
            .ToArbitrary();

        Prop
            .ForAll(gen, list => list
                .Select(x => x.Item1)
                .IsEvenlySpread(size, 0.05))
            .QuickCheckThrowOnFailure();
    }

    [Property(Arbitrary = [typeof(CombinationsGenerator),])]
    public Property Generator2(Combination combination)
    {
        var lengthOfItemsIsSameAsLengthOfSizes = combination.Item.Length == combination.Sizes.Length;
        var itemIsSmallerThanSize = combination
            .Item
            .Zip(combination.Sizes)
            .All(t => t.First < t.Second);
        var sizeIsGreaterOrEqualToTwo = combination
            .Sizes
            .All(x => 2 <= x);
        var itemsIsGreaterOrEqualToZero = combination
            .Item
            .All(x => 0 <= x)
            .Label("Items is greater or equal to zero");

        return itemIsSmallerThanSize
            .And(sizeIsGreaterOrEqualToTwo)
            .And(itemsIsGreaterOrEqualToZero)
            .And(lengthOfItemsIsSameAsLengthOfSizes);
    }
}