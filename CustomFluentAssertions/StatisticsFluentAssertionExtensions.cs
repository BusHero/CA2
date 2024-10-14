using FluentAssertions.Collections;
using FluentAssertions.Execution;

namespace CustomFluentAssertions;

public static class StatisticsFluentAssertionExtensions
{
    public static AndConstraint<GenericCollectionAssertions<int>> BeEvenlySpread(
        this GenericCollectionAssertions<int> assertion,
        int numberOfElements,
        double tolerance,
        string because = "",
        params object[] becauseArgs)
    {
        Execute.Assertion
            .BecauseOf(because, becauseArgs)
            .WithExpectation("Expected {context:collection} to be evenly distributed{reason}, ")
            .Given(() => assertion.Subject)
            .ForCondition(subject => subject is not null)
            .FailWith("but found <null>")
            .Then
            .ForCondition(x => x.IsEvenlySpread(numberOfElements, tolerance))
            .FailWith("but found non-even distribution")
            .Then
            .ClearExpectation();

        return new AndConstraint<GenericCollectionAssertions<int>>(assertion);
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

        return IsEvenlyDistributed(
            array,
            count,
            numberOfElements,
            tolerance);
    }

    public static bool IsEvenlySpread<T>(
        this IEnumerable<T> range,
        int numberOfElements,
        double tolerance) where T : notnull
    {
        var count = 0;

        var dict = new Dictionary<T, int>(numberOfElements);

        foreach (var i in range)
        {
            dict.TryGetValue(i, out var count2);
            count2++;
            dict[i] = count2;
            count++;
        }

        return IsEvenlyDistributed(dict.Values, count, numberOfElements, tolerance);
    }

    private static bool IsEvenlyDistributed(
        IReadOnlyCollection<int> values,
        int count,
        int numberOfElements,
        double tolerance)
    {
        var distribution = 1.0 / numberOfElements;

        var intermediateResult = values
            .Select(x => (double)x)
            .Select(x => x / count)
            .ToList();

        return intermediateResult
            .All(x => Math.Abs(x - distribution) < tolerance);
    }
}