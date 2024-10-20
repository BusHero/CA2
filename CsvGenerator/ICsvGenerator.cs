namespace CsvGenerator;

using System.IO;
using System.Threading.Tasks;

public interface ICsvGenerator
{
    Task GenerateAsync(
        TextWriter writer, 
        int rowsCount, 
        string[][] columns);
}