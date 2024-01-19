using BenchmarkDotNet.Running;
using Microsoft.CodeAnalysis;

namespace DenevCloud.Core.Unmanaged.Benchmarks;

public class Program
{
    public unsafe static void Main()
    {
        BenchmarkRunner.Run<Benchmarks>();
    }
}