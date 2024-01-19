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

    [Benchmark]
    public void CreateUnmanagedArrayOf5ObjectsAndChangeValue()
    {
        var unmanaged = new UnmanagedArray<Person>(5);

        unmanaged[0] = new Person()
        {
            Age = 99,
            Id = Guid.NewGuid(),
            Name = "Mylo 1"
        };

        unmanaged[1] = new Person()
        {
            Age = 100,
            Id = Guid.NewGuid(),
            Name = "Mylo 2"
        };

        unmanaged[2] = new Person()
        {
            Age = 101,
            Id = Guid.NewGuid(),
            Name = "Mylo 3"
        };

        unmanaged[3] = new Person()
        {
            Age = 102,
            Id = Guid.NewGuid(),
            Name = "Mylo 4"
        };

        unmanaged[4] = new Person()
        {
            Age = 103,
            Id = Guid.NewGuid(),
            Name = "Mylo 5"
        };
    }
}