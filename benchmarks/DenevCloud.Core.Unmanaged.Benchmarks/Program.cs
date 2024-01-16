using BenchmarkDotNet.Running;

namespace DenevCloud.Core.Unmanaged.Benchmarks;

public class Program
{
    public unsafe static void Main()
    {
        using var unmanaged = new UnmanagedObject<Person>();
        Console.WriteLine(unmanaged.Value.Age);
        var myStruct2 = new Person() { Age = 200 };
        unmanaged.Update(&myStruct2);
        Console.WriteLine(unmanaged.Value.Age);

        unmanaged.Dispose();

        BenchmarkRunner.Run<Benchmarks>();
    }
}