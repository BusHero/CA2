namespace TestUtils.Tests;

public sealed class GeneratorsTests
{
    [Property(Arbitrary = [typeof(Generators)])]
    public Property ColumnSizeIsBiggerOrEqualToTwo(ColumnSize column)
    {
        return (2 <= column.Get).ToProperty();
    }
    
    [Property(Arbitrary = [typeof(Generators)])]
    public Property ColumnSizeIsLessThan20(ColumnSize column)
    {
        return (column.Get <= 20).ToProperty();
    }
}