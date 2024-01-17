# Unmanaged Objects in C# made easy

DenevCloud.Core.Unmanaged API allows for allocating unmanaged objects on the heap and deallocate them then they get out of scope. `UnmanagedObject<T>` implements the `IDisposable` interface and follows this pattern.

# Examples

```csharp
//Example 1
var unmanaged = new UnmanagedObject<MyStruct>(); // Creates default(MyStruct)
unmanaged.Value = new MyStruct() { Age = 99 };
ref var myStruct = ref unmanaged.RefValue;
unmanaged. Dispose(); //never forget to dispose if it's not under 'using' scope

//Example 2
using var unmanaged = new UnmanagedObject<MyStruct>();
MyStruct* handle = unmanaged.GetHandle();
*handle = new MyStruct() { Age = 99 }; //The value within unmanaged also changes since it's pointer

//Example 3
using var unmanaged = new UnmanagedObject<MyStruct>();
var myStruct2 = new MyStruct();
unmanaged.Update(&myStruct2);
```

# Since Version 1.0.1
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
