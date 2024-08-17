using FluentAssertions;
using FluentAssertions.Collections;
using FluentAssertions.Execution;

namespace CA2.Tests;

internal static class StatisticsFluentAssertionExtensions
{
    public static AndConstraint<GenericCollectionAssertions<int>> BeEvenlySpread(
        this GenericCollectionAssertions<int> foo, 
        int numberOfElements, 
        double tolerance,
        string because = "", 
        params object[] becauseArgs)
    {
        Execute.Assertion
            .BecauseOf(because, becauseArgs)
            .WithExpectation("Expected {context:collection} to be evenly distributed{reason}, ")
            .Given(() => foo.Subject)
            .ForCondition(subject => subject is not null)
            .FailWith("but found <null>")
            .Then
            .ForCondition(x => x.IsEvenlySpread(numberOfElements, tolerance))
            .FailWith("but found non-even distribution")
            .Then
            .ClearExpectation();
        
        return new AndConstraint<GenericCollectionAssertions<int>>(foo);
    }
    
    public static bool IsEvenlySpread(
        this IEnumerable<int> range,
        int numberOfElements, 
        double tolerance)
    {
        var count = 0;

        var array = new int[numberOfElements];

        foreach (var i in range)
        {
            array[i]++;
            count++;
        }

        var distribution = 1.0 / numberOfElements;
        var intermediateResult = array
            .Select(x => (double)x)
            .Select(x => x  / count)
            .ToList();
        
        return intermediateResult
            .All(x => Math.Abs(x - distribution) < tolerance);
    }
}