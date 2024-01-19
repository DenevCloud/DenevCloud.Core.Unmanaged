using System.Runtime.CompilerServices;

namespace DenevCloud.Core.Unmanaged.Examples;

public static class Program
{
    static unsafe void Main()
    {
        Settings.UseAllocationManager = false;

        //Example 1
        var unmanaged = new UnmanagedObject<Person>(); // Creates default(MyStruct)
        unmanaged.Value = new Person() { Age = 99 };
        ref var myStruct = ref unmanaged.RefValue;
        unmanaged.Dispose(); //never forger to dispose if it's not under 'using' scope

        Console.WriteLine(myStruct.Age);

        //Example 2
        using var unmanaged2 = new UnmanagedObject<Person>();
        Person* handle = unmanaged2.GetHandle();
        *handle = new Person() { Age = 99 }; //The value within unmanaged also changes since it's pointer

        //Example 3
        using var unmanaged3 = new UnmanagedObject<Person>();
        var myStruct2 = new Person();
        unmanaged3.Update(&myStruct2);

        AllocExample();
        AllocUnmanagedHeapExample();
    }

    static void AllocExample()
    {
        for(long i = 0; i < 10_000_000; i++)
        {
            long a = Random.Shared.Next(1, 10_000_000);
            long b = Random.Shared.Next(1, 10_000_000);

            SomeSender(a, b);
        }
    }

    static void AllocUnmanagedHeapExample()
    {
        for (long i = 0; i < 10_000_000; i++)
        {
            var a = new UnmanagedObject<long>();
            a.Value = Random.Shared.Next(1, 10_000_000);

            var b = new UnmanagedObject<long>();
            b.Value = Random.Shared.Next(1, 10_000_000);

            SomeSender(a, b);

            a.Dispose();
            b.Dispose();
        }
    }

    static void SomeSender(long val1, long val2)
    {
        //Do something
        _ = val1 + val2;
    }
}