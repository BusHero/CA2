namespace CA2.Tests.Utils;

using Xunit.Abstractions;

public sealed record Combination : IXunitSerializable
{
    public int[] Item { get; set; } = null!;

    public int[] Sizes { get; set; } = null!;

    public void Deserialize(IXunitSerializationInfo info)
    {
        Item = info.GetValue<int[]>(nameof(Item));
        Sizes = info.GetValue<int[]>(nameof(Sizes));
    }

    public void Serialize(IXunitSerializationInfo info)
    {
        info.AddValue(nameof(Item), Item);
        info.AddValue(nameof(Sizes), Sizes);
    }

    public override string ToString() 
        => $$"""Combination { Item = [{{string.Join(", ", Item)}}], Sizes = [{{string.Join(", ", Sizes)}}] }""";
}