using System.Numerics;

using GeneratorLibrary;

namespace CA2.Tests.GeneratorTests;

public sealed class WriteToBufferTests
{
    [Property]
    public Property SpecifiedNumberOfBytesIsWrittenToTheStream(
        PositiveInt size)
    {
        var stream = new MemoryStream();

        Generator.TryWriteToBuffer(
            stream,
            [BigInteger.One],
            size.Item);

        var array = stream.ToArray();

        return (array.Length == size.Item)
            .Label($"{array.Length} == {size.Item}");
    }

    [Property]
    public Property FunctionReturnsTrue(
        PositiveInt size)
    {
        var stream = new MemoryStream();

        return Generator
            .TryWriteToBuffer(
                stream,
                [BigInteger.One],
                size.Item)
            .ToProperty();
    }

    [Property]
    public Property SizeIsSmallerThanNumber(
        byte[] bytes,
        PositiveInt size)
    {
        var prop = () =>
        {
            var stream = new MemoryStream();

            var number = new BigInteger(bytes);

            var result = Generator.TryWriteToBuffer(
                stream,
                [number],
                size.Item);

            return !result;
        };

        return prop.When(size.Item < bytes.Length);
    }

    [Property]
    public Property RecoverNumberFromStream(
        BigInteger number)
    {
        var stream = new MemoryStream();

        Generator.TryWriteToBuffer(
            stream,
            [number],
            number.GetByteCount());

        var newNumber = new BigInteger(stream.ToArray());

        return (newNumber == number)
            .Label($"{newNumber} == {number}");
    }

    [Property]
    public Property RecoverNumberFromStream2(
        NonEmptyArray<BigInteger> numbers)
    {
        var stream = new MemoryStream();

        var bytesPerNumber = numbers
            .Item
            .Max(BigInteger.Abs)
            .GetByteCount();

        Generator.TryWriteToBuffer(
            stream,
            numbers.Item,
            bytesPerNumber);

        var bufferSize = stream.ToArray()
            .Length;

        var expectedSize = bytesPerNumber * numbers.Item.Length;

        return (bufferSize == expectedSize)
            .ToProperty();
    }

    [Property]
    public Property EmptyArrayWriteNothingToStream(PositiveInt size)
    {
        var stream = new MemoryStream();

        Generator.TryWriteToBuffer(
            stream,
            [],
            size.Item);

        var bufferSize = stream.ToArray()
            .Length;

        return (bufferSize == 0)
            .ToProperty();
    }

    [Property]
    public Property EmptyArrayReturnsTrue(PositiveInt size)
    {
        var stream = new MemoryStream();

        return Generator.TryWriteToBuffer(
                stream,
                [],
                size.Item)
            .ToProperty();
    }
}
