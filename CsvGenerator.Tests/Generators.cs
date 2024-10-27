using System.Diagnostics.CodeAnalysis;

namespace CsvGenerator.Tests;

public static class Generators
{
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    public static Arbitrary<Column> Generator()
        => Arb.Default.PositiveInt()
            .Generator
            .Select(x => 2 <= x.Get ? x.Get : 2)
            .Select(x => new Column(x))
            .ToArbitrary();
}