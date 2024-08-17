using FsCheck;

namespace CA2.Tests;

public class Generators
{
    public static Gen<(int, int)> Generator3 { get; } = Gen
        .Sized(size => Gen
            .Choose(0, size < 2 ? 1 : size - 1)
            .Select(x => (x, size < 2 ? 2 : size)));

    public static Arbitrary<(int, int)> TupleArbitrary()
    {
        return Generator3
            .ToArbitrary();
    }

    public static Arbitrary<Combination> CombinationArbitrary()
    {
        return Generator3
            .ArrayOf()
            .Select(items => new Combination
            {
                Item = items.Select(x => x.Item1).ToArray(),
                Sizes = items.Select(x => x.Item2).ToArray()
            })
            .ToArbitrary();
    }
}