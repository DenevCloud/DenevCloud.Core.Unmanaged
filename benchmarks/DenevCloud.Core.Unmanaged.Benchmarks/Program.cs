using BenchmarkDotNet.Running;

namespace DenevCloud.Core.Unmanaged.Benchmarks;

public class Program
{
    public unsafe static void Main()
    {        
        BenchmarkRunner.Run<Benchmarks>();
    }
}