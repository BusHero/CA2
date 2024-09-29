namespace CA2.Tests.GeneratorTests;

using System.Numerics;

using Utils;

public sealed class GetBytesTests
{
    [Property(Arbitrary = [typeof(CombinationsGenerator)])]
    public Property ArrayProducedIsEnoughToStoreTheBiggestNumber(Combination combination)
    {
        var bytes = GeneratorLibrary.Generator
            .GetBytes(combination.Item, combination.Sizes);

        var biggestNumber = TestUtils.CalculateMaximumNumber(combination.Sizes);
        var bytesCount = biggestNumber.GetByteCount();

        return (bytesCount == bytes.Length).ToProperty();
    }

    [Property(Arbitrary = [typeof(CombinationsGenerator)])]
    public Property CanConvertBytesBackToBigNumber(Combination combination)
    {
        var number = GeneratorLibrary.Generator.Generate(
            combination.Item,
            combination.Sizes);
        var bytes = GeneratorLibrary.Generator
            .GetBytes(combination.Item, combination.Sizes);
        var newNumber = new BigInteger(bytes);

        return (newNumber == number)
            .Label($"{number} == {newNumber}(0x{string.Join("", bytes.Select(x => x.ToString("x")))})");
    }
}