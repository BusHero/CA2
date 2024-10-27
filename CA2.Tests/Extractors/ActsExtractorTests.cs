namespace CA2.Tests.Extractors;

using CA2.Extractors;

using TestUtils;

public sealed class ActsExtractorTests
{
    public ActsExtractorTests()
        => Arb.Register<Generators>();

    [Property]
    public Property ResultContainsExpectedNumberOfRows(
        PositiveInt rows,
        ColumnSize columnSize)
    {
        using var stream = new MemoryStream();
        using var writer = new StreamWriter(stream);
        new ActsGenerator.ActsGenerator()
            .WithRows(rows.Get)
            .WithColumn(columnSize.Get)
            .GenerateAsync(writer)
            .Wait();

        var extractor = new ActsExtractor();
        stream.Position = 0;
        using var reader = new StreamReader(stream);
        var report = extractor.ExtractAsync(reader).Result;

        return (report.Length == rows.Get)
            .Label($"{report.Length} == {rows.Get}");
    }

    [Property]
    public Property ValuesAreInTheExpectedRange(
        PositiveInt rows,
        NonEmptyArray<ColumnSize> columns)
    {
        using var stream = new MemoryStream();
        using var writer = new StreamWriter(stream);
        columns.Get.Aggregate(
                new ActsGenerator.ActsGenerator().WithRows(rows.Get),
                (gen, column) => gen.WithColumn(column.Get))
            .GenerateAsync(writer)
            .Wait();

        stream.Position = 0;
        using var reader = new StreamReader(stream);

        var extractor = new ActsExtractor();
        var report = extractor.ExtractAsync(reader).Result.Pivot();

        return report
            .Select((row, i) => row.All(x => 0 <= x && x < columns.Get[i].Get))
            .All(x => x)
            .ToProperty();
    }

    [Property]
    public Property ResultContainsExpectedNumberOfValuesPerColumn(
        PositiveInt rows,
        NonEmptyArray<ColumnSize> columns)
    {
        using var stream = new MemoryStream();
        using var writer = new StreamWriter(stream);
        columns.Get.Aggregate(
                new ActsGenerator.ActsGenerator().WithRows(rows.Get),
                (gen, column) => gen.WithColumn(column.Get))
            .GenerateAsync(writer)
            .Wait();

        stream.Position = 0;
        using var reader = new StreamReader(stream);

        var extractor = new ActsExtractor();
        var report = extractor.ExtractAsync(reader).Result;

        return report
            .All(row => row.Length == columns.Get.Length)
            .ToProperty();
    }

    [Property]
    public Property SpaceAtTheEndIsHandledCorrectly(
        PositiveInt rows,
        NonEmptyArray<ColumnSize> columns)
    {
        using var stream = new MemoryStream();
        using var writer = new StreamWriter(stream);
        columns.Get.Aggregate(
                new ActsGenerator.ActsGenerator().WithRows(rows.Get).AddEmptyLineAtEnd(),
                (gen, column) => gen.WithColumn(column.Get))
            .GenerateAsync(writer)
            .Wait();

        stream.Position = 0;
        using var reader = new StreamReader(stream);

        var extractor = new ActsExtractor();
        var report = extractor.ExtractAsync(reader).Result;

        return (report.Length == rows.Get).ToProperty();
    }

    [Property]
    public Property WhiteSpaceIsHandledCorrectly(
        PositiveInt rows,
        NonEmptyArray<ColumnSize> columns)
    {
        using var stream = new MemoryStream();
        using var writer = new StreamWriter(stream);
        columns.Get.Aggregate(
                new ActsGenerator.ActsGenerator().WithRows(rows.Get).AddWhiteSpace(),
                (gen, column) => gen.WithColumn(column.Get))
            .GenerateAsync(writer)
            .Wait();

        stream.Position = 0;
        using var reader = new StreamReader(stream);

        var extractor = new ActsExtractor();
        var report = extractor.ExtractAsync(reader).Result;

        return report
            .All(row => row.Length == columns.Get.Length)
            .ToProperty();
    }

    [Property]
    public Property MaskedValuesAreHandledCorrectly(
        PositiveInt rows,
        NonEmptyArray<ColumnSize> columns)
    {
        using var stream = new MemoryStream();
        using var writer = new StreamWriter(stream);
        columns.Get.Aggregate(
                new ActsGenerator.ActsGenerator().WithRows(rows.Get).IncludeMaskForIrrelevantValues(),
                (gen, column) => gen.WithColumn(column.Get))
            .GenerateAsync(writer)
            .Wait();

        stream.Position = 0;
        using var reader = new StreamReader(stream);

        var extractor = new ActsExtractor();
        var report = extractor.ExtractAsync(reader).Result.Pivot();

        return report
            .Select((row, i) => row.All(x => 0 <= x && x < columns.Get[i].Get))
            .All(x => x)
            .ToProperty();
    }
}