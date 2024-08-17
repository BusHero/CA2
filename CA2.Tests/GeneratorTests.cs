using FsCheck;
using FsCheck.Xunit;

namespace CA2.Tests;

public sealed class GeneratorTests
{
    [Property(Arbitrary = [typeof(Generators)])]
    public Property FirstItemIsSmallerThanSecondItem(
        (int, int) x)
    {
        return (x.Item1 < x.Item2).Label("First item is smaller than Second item")
            .And(0 <= x.Item1).Label("First item is bigger than 0")
            .And(2 <= x.Item2).Label("Second item is at least 2");
    }
    
    [Property(Arbitrary = [typeof(Generators)])]
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
        var gen = Generators
            .Generate()
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

        var gen = Generators
            .Generate()
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

        var gen = Generators
            .Generate()
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

    [Fact]
    public void Generator2()
    {
        // Arbitrary<(int[], int[])> arb;
        //
        // Prop.ForAll(arb, x =>
        // {
        //     return x.Item1
        //         .Zip(x.Item2, (fst, snd) => fst < snd)
        //         .All(y => y);
        // });
    }
    
    // [Fact]
    // public void Gen()
    // {
    //     Arbitrary<(int[], int[])> gen;
    //
    //     Prop.ForAll(gen, x =>
    //     {
    //         var (first, second) = x;
    //         return first.Length == second.Length;
    //     });
    // }
    //
    // [Fact]
    // public void FirstElementIsBiggerThanSecond()
    // {
    //     Arbitrary<(int[], int[])> gen;
    //
    //     Prop.ForAll(gen, x =>
    //     {
    //         var (first, second) = x;
    //         return first.Zip(second).All(t => t.First < t.Second);
    //     });
    // }
    //
    // [Fact]
    // public void SecondElementsAreBiggerThan0()
    // {
    //     Arbitrary<(int[], int[])> gen;
    //
    //     Prop.ForAll(gen, x =>
    //     {
    //         var (_, second) = x;
    //         return second.All(u => 2 <= u);
    //     });
    // }
    //
    // [Fact]
    // public void FirstElementIsBiggerThanZero()
    // {
    //     Arbitrary<(int[], int[])> gen;
    //
    //     Prop.ForAll(gen, x =>
    //     {
    //         var (first, _) = x;
    //         return first.All(u => 0 <= u);
    //     });
    // }
}

