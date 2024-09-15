namespace CA2.Tests;

using GeneratorLibrary;

public sealed class CombiningStuffTogetherTests
{
    [Fact]
    public void OutputFileGetsWritten()
    {
        const string inputFile = "test.csv";
        const string outputFile = "test.cca";
        
        var fileSystem = new FakeFileSystem();
        fileSystem.Add(inputFile, string.Empty);

        var instance = new ClassThatDoesStuff(fileSystem);
        instance.DoStuff(inputFile);

        var result = fileSystem.ContainsFile(outputFile);

        result.Should().BeTrue();
    }
}

public sealed class MockFileSystemTests
{
    [Theory, AutoData]
    public void AddedFileIsReportedAsAdded(
        string inputFile, string content)
    {
        var fileSystem = new FakeFileSystem();
        
        fileSystem.Add(inputFile, content);

        fileSystem
            .ContainsFile(inputFile)
            .Should()
            .BeTrue();
    }
    
    [Theory, AutoData]
    public void FileThatDoesNotExistIsReportedAsNotAdded(
        string inputFile)
    {
        var fileSystem = new FakeFileSystem();
        
        fileSystem
            .ContainsFile(inputFile)
            .Should()
            .BeFalse();
    }

    [Theory, AutoData]
    public void CanReadFileContent(string filename, string content)
    {
        var fileSystem = new FakeFileSystem();
        
        fileSystem.Add(filename, content);

        var savedContent = fileSystem.ReadFile(filename);
        
        savedContent.Should().Be(content);
    }
}