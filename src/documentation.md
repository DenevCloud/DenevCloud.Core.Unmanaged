# Unmanaged Objects in C# made easy

DenevCloud.Core.Unmanaged API allows for allocating unmanaged objects on the heap and deallocate them then they get out of scope. `UnmanagedObject<T>` implements the `IDisposable` interface and follows this pattern.

# Examples

```csharp
//Example 1
var unmanaged = new UnmanagedObject<MyStruct>(); // Creates default(MyStruct)
unmanaged.Value = new MyStruct() { Age = 99 };
ref var myStruct = ref unmanaged.RefValue;
unmanaged. Dispose(); //never forger to dispose if it's not under 'using' scope

//Example 2
using var unmanaged = new UnmanagedObject<MyStruct>();
T* handle = unmanaged.GetHandle();
*handle = new MyStruct() { Age = 99 }; //The value within unmanaged also changes since it's pointer

//Example 3
using var unmanaged = new UnmanagedObject<MyStruct>();
var myStruct2 = new MyStruct();
unmanaged.Update(&myStruct2);
```

# Benchmarks
![UnmanagedObject Benchmarks](https://cdn.denevcloud.net/milen-denev/unmanagedBenchmarks.jpg)