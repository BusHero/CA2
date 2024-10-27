using System.Diagnostics.CodeAnalysis;

using FsCheck;

namespace TestUtils;

public sealed class Generators
{
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    public static Arbitrary<ColumnSize> ColumnSize()
        => Arb.Default.PositiveInt().Generator
            .Select(x => x.Get)
            .Select(x => x % 20)
            .Select(x => 2 <= x ? x : 2)
            .Select(x => new ColumnSize(x))
            .ToArbitrary();
}