using FluentAssertions;
using FsCheck;
using FsCheck.Xunit;

namespace CA2.Tests;

public sealed class GeneratorTests
{
    [Property(Arbitrary = [typeof(Generators)])]
    public Property FirstItemIsSmallerThanSecondItem(
        (int, int) x)
    {
        return (x.Item1 < x.Item2).ToProperty();
    }

    [Property(Arbitrary = [typeof(Generators)])]
    public Property FirstItemIsBiggerThanZero(
        (int, int) x)
    {
        return (0 <= x.Item1).ToProperty();
    }

    [Fact]
    public void SizesAreDifferent()
    {
        var gen = Generators
            .Generate()
            .Generator
            .ArrayOf(1000)
            .ToArbitrary();

        Prop.ForAll(gen, items => items
                .Select(x => x.Item1)
                .All(x => x == items[0].Item1) == false)
            .VerboseCheckThrowOnFailure();
    }

    [Property]
    public void SecondItemIsEqualToSize(PositiveInt size)
    {
        var expectedSize = size.Item < 2 
            ? 2 
            : size.Item;

        var gen = Generators
            .Generate()
            .Generator
            .ArrayOf(10)
            .Resize(size.Item)
            .ToArbitrary();
        
        Prop
            .ForAll(gen, items =>
            {
                return items
                    .Select(x => x.Item2)
                    .All(x => x == expectedSize);
            })
            .QuickCheckThrowOnFailure();
    }
}