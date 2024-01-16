using BenchmarkDotNet.Running;

namespace DenevCloud.Core.Unmanaged.Benchmarks;

public class Program
{
    public static void Main()
    {
        BenchmarkRunner.Run<Benchmarks>();
    }
}