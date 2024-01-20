using Microsoft.Extensions.Primitives;
using static DenevCloud.Core.Unmanaged.Tests.UnmanagedObjectTest;

namespace DenevCloud.Core.Unmanaged.Tests;

public unsafe class UnmanagedArrayTests
{
    [Fact]
    public void InitArrayOf5AndRead()
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

        Person[] person = unmanaged.Array;

        Assert.True(
            person[0].Age == 99 && 
            person[1].Age == 100 &&
            person[2].Age == 101 &&
            person[3].Age == 102 &&
            person[4].Age == 103);
    }

    [Fact]
    public void InitDefaultArrayAndRead()
    {
        var unmanaged = new UnmanagedArray<Person>();

        unmanaged[0] = new Person()
        {
            Age = 99,
            Id = Guid.NewGuid(),
            Name = "Mylo 1"
        };

        Person[] person = unmanaged.Array;

        Assert.True(
            person[0].Age == 99);
    }

    [Fact]
    public void InitArrayOf5AndSortBy()
    {
        var unmanagedArray = new UnmanagedArray<Person>(5);

        unmanagedArray[3] = new Person()
        {
            Id = Guid.NewGuid(),
            Name = "Mylo 1",
            Age = 99,
        };

        unmanagedArray[1] = new Person()
        {
            Id = Guid.NewGuid(),
            Name = "Mylo 2",
            Age = 100,
        };

        unmanagedArray[0] = new Person()
        {
            Id = Guid.NewGuid(),
            Name = "Mylo 3",
            Age = 101,
        };

        unmanagedArray[4] = new Person()
        {
            Id = Guid.NewGuid(),
            Name = "Mylo 4",
            Age = 102,
        };

        unmanagedArray[2] = new Person()
        {
            Id = Guid.NewGuid(),
            Name = "Mylo 5",
            Age = 103,
        };

        var fieldName = new StringValues("Age");

        unmanagedArray.SortBy<byte>(ref fieldName);

        Person[] person = unmanagedArray.Array;

        Assert.True(
           person[0].Age == 99 &&
           person[1].Age == 100 &&
           person[2].Age == 101 &&
           person[3].Age == 102 &&
           person[4].Age == 103);
    }

    [Fact]
    public void Dispose()
    {
        var unmanaged = new UnmanagedArray<Person>();
        unmanaged.Dispose();
        Assert.True(unmanaged.Disposed == true && new IntPtr(unmanaged.GetHandle()) == IntPtr.Zero);
    }
}