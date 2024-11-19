namespace CA2.Compression;

using System.IO;

public interface ICompressor
{
    Task WriteCcaAsync(
        int[][] csv,
        IReadOnlyCollection<int> sizes,
        Stream stream,
        CancellationToken cancellationToken = default);
    
    public void WriteMetadata(
        long numberOfRows,
        IReadOnlyCollection<int> sizes,
        byte interactionStrength,
        Stream stream);
}
