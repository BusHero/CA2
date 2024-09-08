namespace CA2.Tests.GeneratorTests;

public sealed class Tests2
{
    [Property(Arbitrary = [typeof(CombinationsGenerator),])]
    public void Foo()
    {
        var stream = new MemoryStream();

        var array = stream.ToArray();

        array.Should()
            .NotBeEmpty();
    }
}
