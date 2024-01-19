# Unmanaged Objects in C# made easy

DenevCloud.Core.Unmanaged API allows for allocating unmanaged objects on the heap and deallocate them then they get out of scope. `UnmanagedObject<T>` implements the `IDisposable` interface and follows this pattern.

# Examples

```csharp
//Object Example 1
var unmanaged = new UnmanagedObject<Person>(); // Creates default(MyStruct)
unmanaged.Value = new Person() { Age = 99 };
ref var myStruct = ref unmanaged.RefValue;
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
```

# Version 1.0.2
Added `UnmanagedArray<T>` which works on similar principle but it's optimized for arrays and allocates a whole memory block for the whole array. Currently not supporting resizing.

# Version 1.0.1
New feature has been added called `AllocationManager` which allows to reuse the blocks of memory that are allocated. This greatly improves performance since no new memory blocks allocations are made.

## Settings

```csharp
using DenevCloud.Core.Unmanaged;

Settings.UseAllocationManager = true; //Default is false
Settings.MaxAllocations = 32; //Default is 16, corresponds to total number of memory blocks allocated
Settings.MaxAllocationLifetime = TimeSpan.FromMinutes(2); //Default is 2 minutes, corresponds to the max lifetime of a memory block (if object disposed)
Settings.ExpiredCheck = TimeSpan.FromSeconds(5); //Default is 5 seconds, corresponds to how ofter the AllocationManager checks for expired memory blocks
```

# Benchmarks
![UnmanagedObject Benchmarks](https://cdn.denevcloud.net/milen-denev/unmanagedBenchmarks.jpg)
