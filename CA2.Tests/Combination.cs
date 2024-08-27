namespace CA2.Tests;

public sealed record Combination
{
    public required int[] Item { get; init; }

    public required int[] Sizes { get; init; }

    public override string ToString()
    {
        return $$"""Combination { Item = [{{string.Join(", ", Item)}}], Sizes = [{{string.Join(", ", Sizes)}}] }""";
    }
}