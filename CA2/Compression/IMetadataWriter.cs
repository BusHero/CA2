namespace CA2.Compression;

using System.IO;

public interface IMetadataWriter
{
    public void Write(
        long numberOfRows,
        IReadOnlyCollection<int> sizes,
        byte interactionStrength,
        Stream stream);
}
