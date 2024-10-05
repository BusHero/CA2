namespace CA2.Tests.Utils;

using Xunit.Abstractions;

public sealed record RealCombination: IXunitSerializable
{
    public int[][] Items { get; set; } = null!;

    public int[] Sizes { get; set; } = null!;

    public void Deserialize(IXunitSerializationInfo info)
    {
        Items = info.GetValue<int[][]>(nameof(Items));
        Sizes = info.GetValue<int[]>(nameof(Sizes));
    }

    public void Serialize(IXunitSerializationInfo info)
    {
        info.AddValue(nameof(Items), Items);
        info.AddValue(nameof(Sizes), Sizes);
    }

    public override string ToString()
    {
        var foo = Items.Select(x => $"[ {string.Join(", ", x)} ]");
        var bar = string.Join(", ", foo);

        return $$"""Combination { Item = [ {{bar}} ], Sizes = [{{string.Join(", ", Sizes)}}] }""";
    }
}

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