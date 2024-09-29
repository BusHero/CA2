namespace CA2.Tests.GeneratorTests;

using System.Numerics;

using Utils;

public sealed class GetBytesTests
{
    private readonly GeneratorLibrary.Generator _generator = new();
    
    [Property(Arbitrary = [typeof(CombinationsGenerator)])]
    public Property ArrayProducedIsEnoughToStoreTheBiggestNumber(Combination combination)
    {
        var bytes = _generator
            .GetBytes(combination.Item, combination.Sizes);

        var biggestNumber = TestUtils.CalculateMaximumNumber(combination.Sizes);
        var bytesCount = biggestNumber.GetByteCount();

        return (bytesCount == bytes.Length).ToProperty();
    }

    [Property(Arbitrary = [typeof(CombinationsGenerator)])]
    public Property CanConvertBytesBackToBigNumber(Combination combination)
    {
        var number = _generator.Generate(
            combination.Item,
            combination.Sizes);
        
        var bytes = _generator
            .GetBytes(combination.Item, combination.Sizes);
        var newNumber = new BigInteger(bytes);

        return (newNumber == number)
            .Label($"{number} == {newNumber}(0x{string.Join("", bytes.Select(x => x.ToString("x")))})");
    }
}