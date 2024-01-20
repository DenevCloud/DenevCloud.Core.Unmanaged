using Microsoft.Extensions.Primitives;
using System.Runtime.CompilerServices;

namespace DenevCloud.Core.Unmanaged.Examples;

public static class Program
{
    static unsafe void Main()
    {
        Settings.UseAllocationManager = false;

        //Object Example 1
        var unmanaged = new UnmanagedObject<Person>(); // Creates default(MyStruct)
        unmanaged.Value = new Person() { Age = 99 };
        var myStruct = unmanaged.Value;
        unmanaged.Dispose(); //never forger to dispose if it's not under 'using' scope

        Console.WriteLine(myStruct.Age);

        //Object Example 2
        using var unmanaged2 = new UnmanagedObject<Person>();
        Person* handle = unmanaged2.GetHandle();
        *handle = new Person() { Age = 99 }; //The value within unmanaged also changes since it's pointer

        //Object Example 3
        using var unmanaged3 = new UnmanagedObject<Person>();
        var myStruct2 = new Person();
        unmanaged3.Update(&myStruct2);

        //Array Example 1
        var unamanagedArray1 = new UnmanagedArray<Person>(5);
        unamanagedArray1[4] = new Person()
        {
            Age = 103,
            Id = Guid.NewGuid(),
            Name = "Mylo 5"
        };
        unamanagedArray1[3] = new Person()
        {
            Age = 103,
            Id = Guid.NewGuid(),
            Name = "Mylo 5"
        };

        Person[] persons = unamanagedArray1.Array;
        Console.WriteLine(persons[4].Age);
        unamanagedArray1.Dispose();

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

        unmanagedArray.Resize(6);

        unmanagedArray[5] = new Person()
        {
            Id = Guid.NewGuid(),
            Name = "Mylo 4",
            Age = 102,
        };

        Person[] person2 = unmanagedArray.Array;

        unmanagedArray.Shrink(5);

        Person[] person3 = unmanagedArray.Array;

        Console.ReadKey();
    }
}