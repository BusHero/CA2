namespace CA2.Tests.GeneratorTests;

using System.Numerics;

using GeneratorLibrary;

using Utils;

public sealed class WriteToBufferTests
{
    private readonly Compressor _compressor = new();

    [Property(Arbitrary = [typeof(CombinationsGenerator)])]
    public Property Foo(Combination combination)
    {
        using var stream = new MemoryStream();
        
        _compressor.Compress(
            combination.Item, 
            combination.Sizes, 
            stream);
        var actualBytes = stream.ToArray();
        
        var lengthBiggerThanZero = (actualBytes.Length > 0).ToProperty();
        var lengthSmallerThan = (actualBytes.Length < combination.Item.Length * 4).ToProperty();
        
        return lengthBiggerThanZero
            .And(lengthSmallerThan)
            .When(combination.Item is not []);
    }

    [Property]
    public Property SpecifiedNumberOfBytesIsWrittenToTheStream(
        PositiveInt size)
    {
        var stream = new MemoryStream();

        _compressor.TryWriteToBuffer(
            stream,
            [BigInteger.One],
            size.Item);

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
            .TryWriteToBuffer(
                stream,
                [BigInteger.One],
                size.Item)
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

            var number = new BigInteger(bytes);

            var result = _compressor.TryWriteToBuffer(
                stream,
                [number],
                size.Item);

            return !result;
        };

        return prop.When(size.Item < bytes.Length);
    }

    [Property]
    public Property NumberIsWrittenToStream(
        BigInteger number)
    {
        var stream = new MemoryStream();

        _compressor.TryWriteToBuffer(
            stream,
            [number],
            number.GetByteCount());

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

        _compressor.TryWriteToBuffer(
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
    public Property MultipleNumberAreWrittenToStream_RecoverNumbers(
        NonEmptyArray<BigInteger> numbers)
    {
        using var stream = new MemoryStream();

        var bytesPerNumber = numbers
            .Item
            .Max(BigInteger.Abs)
            .GetByteCount();

        _compressor.TryWriteToBuffer(
            stream,
            numbers.Item,
            bytesPerNumber);

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

        _compressor.TryWriteToBuffer(
            stream,
            [],
            size.Item);

        var bufferSize = stream.ToArray()
            .Length;

        return (bufferSize == 0)
            .ToProperty();
    }

    [Property]
    public Property EmptyArray_ReturnsTrue(PositiveInt size)
    {
        var stream = new MemoryStream();

        return _compressor.TryWriteToBuffer(
                stream,
                [],
                size.Item)
            .ToProperty();
    }
}