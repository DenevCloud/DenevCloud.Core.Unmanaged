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
        Person[] persons = unamanagedArray1.Array;
        Console.WriteLine(persons[4].Age);
        unamanagedArray1.Dispose();
    }
}