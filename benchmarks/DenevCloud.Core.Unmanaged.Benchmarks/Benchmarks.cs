using BenchmarkDotNet.Attributes;

namespace DenevCloud.Core.Unmanaged.Benchmarks;

[MemoryDiagnoser]
public unsafe class Benchmarks
{
    public static Person Person2 = new Person() 
    {
        Age = 20,
        Name = "Not Mylo",
        Id = Guid.NewGuid()
    };

    public static UnmanagedObject<Person> Unmanaged = new();

    public Benchmarks() {}

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