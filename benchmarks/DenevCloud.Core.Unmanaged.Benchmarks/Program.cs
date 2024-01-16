using BenchmarkDotNet.Running;
using System.Runtime.InteropServices;

namespace DenevCloud.Core.Unmanaged.Benchmarks;

public class Program
{
    public unsafe static void Main()
    {
        
        BenchmarkRunner.Run<Benchmarks>();
    }
}