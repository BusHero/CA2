namespace GeneratorLibrary;

public interface ICsvCompressor
{
    // ReSharper disable once UnusedMember.Global
    byte[] Compress(string[][] csv);
}