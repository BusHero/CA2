namespace CA2.Tests.Utils;

using System.Diagnostics.CodeAnalysis;

public sealed class CombinationsGenerator
{
    public static Gen<(int, int)> Generator { get; } = Gen
        .Sized(size => Gen
            .Choose(0, size < 2 ? 1 : size - 1)
            .Select(x => (x, size < 2 ? 2 : size)));

    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    public static Arbitrary<(int, int)> TupleArbitrary()
    {
        return Generator
            .ToArbitrary();
    }

    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    public static Arbitrary<Combination> CombinationArbitrary()
        => Generator
            .ArrayOf()
            .Where(x => x is not [])
            .Select(items => new Combination
            {
                Item = items.Select(x => x.Item1).ToArray(),
                Sizes = items.Select(x => x.Item2).ToArray(),
            })
            .ToArbitrary();
}