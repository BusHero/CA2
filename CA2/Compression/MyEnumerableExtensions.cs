namespace CA2.Compression;

using System.Numerics;

public static class MyEnumerableExtensions
{
    public static BigInteger Product(this IEnumerable<int> numbers)
        => numbers.Aggregate(BigInteger.One, (x, y) => x * y);
}