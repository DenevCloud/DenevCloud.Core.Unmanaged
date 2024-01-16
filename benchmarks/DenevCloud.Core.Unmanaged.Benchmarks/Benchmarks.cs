using BenchmarkDotNet.Attributes;
using System.Drawing;
using System.Runtime.InteropServices;

namespace DenevCloud.Core.Unmanaged.Benchmarks;

[MemoryDiagnoser]
[DisassemblyDiagnoser]
public unsafe class Benchmarks
{
    public static Person Person2 = new Person() 
    {
        Age = 20,
        Name = "Not Mylo",
        Id = Guid.NewGuid()
    };

    public static UnmanagedObject<Person> Unmanaged = new();

    public Benchmarks()
    {
    }

    [Benchmark]
    public void TestAlloc()
    {
        
    }

    [Benchmark]
    public void CreateAndDisposeUnmanagedObject()
    {
        using var unmanaged = new UnmanagedObject<Person>();
    }

    [Benchmark]
    public void ChangeValue()
    {
        Unmanaged.Value = Person2;
    }
}