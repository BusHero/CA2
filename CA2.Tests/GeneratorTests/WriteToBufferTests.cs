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
            BigInteger.One,
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

        return Generator.TryWriteToBuffer(
                stream,
                BigInteger.One,
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
                number,
                size.Item);

            return !result;
        };

        return prop.When(size.Item < bytes.Length);
    }
}
