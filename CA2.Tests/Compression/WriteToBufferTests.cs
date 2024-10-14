namespace CA2.Tests.Compression;

using System.Numerics;

using GeneratorLibrary.Compression;

public sealed class WriteToBufferTests
{
    private readonly Compressor _compressor = new();

    [Property]
    public Property SpecifiedNumberOfBytesIsWrittenToTheStream(
        PositiveInt size)
    {
        var stream = new MemoryStream();

        _compressor.TryWriteToBufferAsync(
            stream,
            [BigInteger.One],
            size.Item).Wait();

        var array = stream.ToArray();

        return (array.Length == size.Item)
            .Label($"{array.Length} == {size.Item}");
    }

    [Property]
    public Property SizeIsEnoughToHoldNumber_ReturnsTrue(
        PositiveInt size)
    {
        var stream = new MemoryStream();

        return _compressor
            .TryWriteToBufferAsync(
                stream,
                [BigInteger.One],
                size.Item)
            .Result
            .ToProperty();
    }

    [Property]
    public Property SizeIsNotEnoughToHoldNumber_ReturnsFalse(
        byte[] bytes,
        PositiveInt size)
    {
        var prop = () =>
        {
            var stream = new MemoryStream();

            var number = new BigInteger(bytes, isUnsigned: true);

            var result = _compressor.TryWriteToBufferAsync(
                stream,
                [number],
                size.Item).Result;

            return !result;
        };

        return prop.When(size.Item < bytes.Length);
    }

    [Property]
    public Property NumberIsWrittenToStream(
        BigInteger number)
    {
        var stream = new MemoryStream();

        _compressor.TryWriteToBufferAsync(
            stream,
            [number],
            number.GetByteCount()).Wait();

        var newNumber = new BigInteger(stream.ToArray());

        return (newNumber == number)
            .Label($"{newNumber} == {number}");
    }

    [Property]
    public Property MultipleNumberAreWrittenToStream_BytesAreMultiplierOfSize(
        NonEmptyArray<BigInteger> numbers)
    {
        var stream = new MemoryStream();

        var bytesPerNumber = numbers
            .Item
            .Max(BigInteger.Abs)
            .GetByteCount();

        _compressor.TryWriteToBufferAsync(
            stream,
            numbers.Item,
            bytesPerNumber).Wait();

        var bufferSize = stream.ToArray()
            .Length;

        var expectedSize = bytesPerNumber * numbers.Item.Length;

        return (bufferSize == expectedSize)
            .ToProperty();
    }

    [Property]
    public Property MultipleNumberAreWrittenToStream_RecoverNumbers(
        NonEmptyArray<BigInteger> numbers)
    {
        using var stream = new MemoryStream();

        var bytesPerNumber = numbers
            .Item
            .Max(BigInteger.Abs)
            .GetByteCount();

        _compressor.TryWriteToBufferAsync(
            stream,
            numbers.Item,
            bytesPerNumber).Wait();

        stream.Position = 0;
        var buffer = new byte[bytesPerNumber];
        var numbers2 = Enumerable
            .Range(0, numbers.Item.Length)
            .Select(_ =>
            {
                var __ = stream.Read(buffer);
                return new BigInteger(buffer);
            })
            .ToArray();

        return numbers2
            .SequenceEqual(numbers.Get)
            .ToProperty();
    }

    [Property]
    public Property EmptyArray_WriteNothingToStream(PositiveInt size)
    {
        var stream = new MemoryStream();

        _compressor.TryWriteToBufferAsync(
            stream,
            [],
            size.Item).Wait();

        var bufferSize = stream.ToArray()
            .Length;

        return (bufferSize == 0)
            .ToProperty();
    }

    [Property]
    public Property EmptyArray_ReturnsTrue(PositiveInt size)
    {
        var stream = new MemoryStream();

        return _compressor.TryWriteToBufferAsync(
                stream,
                [],
                size.Item).Result
            .ToProperty();
    }
}