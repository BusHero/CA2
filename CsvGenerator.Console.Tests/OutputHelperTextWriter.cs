namespace CsvGenerator.Console.Tests;

using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Xunit.Abstractions;

public class OutputHelperTextWriter(ITestOutputHelper outputHelper) : TextWriter
{
    public override Encoding Encoding => Encoding.UTF8;

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
    public override async Task WriteLineAsync() => WriteLine();

    public override async Task WriteLineAsync(char value) => WriteLine(value);

    public override async Task WriteLineAsync(char[] buffer, int index, int count) => WriteLine(buffer, index, count);

    public override async Task WriteLineAsync(ReadOnlyMemory<char> buffer, CancellationToken cancellationToken = default) => WriteLine(buffer);

    public override async Task WriteLineAsync(string? value) => WriteLine(value);

    public override async Task WriteLineAsync(StringBuilder? value, CancellationToken cancellationToken = default) => WriteLine(value);
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously

    public override void WriteLine(string? value) => outputHelper.WriteLine(value);

    public override void WriteLine(object? value) => outputHelper.WriteLine(value?.ToString());
    public override void WriteLine() => outputHelper.WriteLine(string.Empty);

    public override void WriteLine(bool value) => outputHelper.WriteLine(value.ToString());
    
    public override void WriteLine(char value) => outputHelper.WriteLine(value.ToString());
    
    public override void WriteLine(char[]? buffer) => outputHelper.WriteLine(new string(buffer));
    
    public override void WriteLine(char[] buffer, int index, int count) => outputHelper.WriteLine(new string(buffer.Skip(index).Take(count).ToArray()));
    
    public override void WriteLine(decimal value) => outputHelper.WriteLine(value.ToString());
    
    public override void WriteLine(double value) => outputHelper.WriteLine(value.ToString());
    
    public override void WriteLine(int value) => outputHelper.WriteLine(value.ToString());
    
    public override void WriteLine(long value) => outputHelper.WriteLine(value.ToString());
    
    public override void WriteLine(ReadOnlySpan<char> buffer) => outputHelper.WriteLine(new string(buffer));
    
    public override void WriteLine(float value) => outputHelper.WriteLine(value.ToString());
    
    public override void WriteLine([StringSyntax("CompositeFormat")] string format, object? arg0) => outputHelper.WriteLine(format, arg0);
    
    public override void WriteLine([StringSyntax("CompositeFormat")] string format, object? arg0, object? arg1) => outputHelper.WriteLine(format, arg0, arg1);
    
    public override void WriteLine([StringSyntax("CompositeFormat")] string format, object? arg0, object? arg1, object? arg2) => outputHelper.WriteLine(format, arg0, arg1, arg2);
    
    public override void WriteLine([StringSyntax("CompositeFormat")] string format, params object?[] arg) => outputHelper.WriteLine(format, arg);
    
    public override void WriteLine(StringBuilder? value) => outputHelper.WriteLine(value?.ToString());
    
    public override void WriteLine(uint value) => outputHelper.WriteLine(value.ToString());
    
    public override void WriteLine(ulong value) => outputHelper.WriteLine(value.ToString());
}