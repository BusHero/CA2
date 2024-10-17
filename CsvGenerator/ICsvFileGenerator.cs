namespace CsvGenerator;

using System.IO;
using System.Threading.Tasks;

public interface ICsvFileGenerator
{
    Task GenerateAsync(StreamWriter writer, int rowsCount, string[][] columns);
}