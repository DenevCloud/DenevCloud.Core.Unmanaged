namespace DenevCloud.Core.Unmanaged.Tests;

#pragma warning disable CS8500

public unsafe partial class UnmanagedObjectTest
{
    [Fact]
    public void InitAndReadEmpty()
    {
        Settings.UseAllocationManager = false;
        var unmanaged = new UnmanagedObject<Person>();
        Assert.True(((Person)unmanaged).Age == 0);
    }

    [Fact]
    public void GetRefValue()
    {
        Settings.UseAllocationManager = false;
        var unmanaged = new UnmanagedObject<Person>();

        ref var test = ref unmanaged.RefValueUnsafe;

        Assert.True(test.Age == 0);
    }

    [Fact]
    public void Dispose()
    {
        Settings.UseAllocationManager = false;
        var unmanaged = new UnmanagedObject<Person>();
        ref var test = ref unmanaged.RefValueUnsafe;
        unmanaged.Dispose();
        Assert.True(unmanaged.Disposed == true && new IntPtr(unmanaged.GetHandle()) == IntPtr.Zero);
    }

    [Fact]
    public void UnmanagedObjectToObject()
    {
        Settings.UseAllocationManager = false;
        var unmanaged = new UnmanagedObject<Person>();
        var test =  unmanaged.Value;
        Assert.True(test.Age == 0);
    }

    [Fact]
    public void ChangeValueOfUnmanagedObject()
    {
        Settings.UseAllocationManager = false;
        var unmanaged = new UnmanagedObject<Person>();
        var newPerson = Person.Create();
        unmanaged.Value = newPerson;

        Person testPerson = unmanaged.Value;

        Assert.True(testPerson.Age == 20);
    }

    [Fact]
    public void UpdateValueFromRefValue()
    {
        Settings.UseAllocationManager = false;
        var unmanaged = new UnmanagedObject<Person>();
        var newPerson = Person.Create();
        unmanaged.Update(ref newPerson);

        Person testPerson = unmanaged.Value;

        Assert.True(testPerson.Age == 20);
    }

    [Fact]
    public void InitAndReadWithPointer_WithAllocationManager()
    {
        Settings.UseAllocationManager = true;
        Person person = Person.Create();
        var _pointer = &person;
        var unmanaged = new UnmanagedObject<Person>(_pointer);
        Person testPerson = unmanaged;
        Assert.True(testPerson.Age == person.Age);
    }

    [Fact]
    public void InitAndReadEmpty_WithAllocationManager()
    {
        Settings.UseAllocationManager = true;
        var unmanaged = new UnmanagedObject<Person>();
        Assert.True(((Person)unmanaged).Age == 0);
    }

    [Fact]
    public void GetRefValue_WithAllocationManager()
    {
        Settings.UseAllocationManager = true;
        var unmanaged = new UnmanagedObject<Person>();
        ref var test = ref unmanaged.RefValueUnsafe;
        Assert.True(test.Age == 0);
    }

    [Fact]
    public void UnmanagedObjectToObject_WithAllocationManager()
    {
        Settings.UseAllocationManager = true;
        var unmanaged = new UnmanagedObject<Person>();
        var test = unmanaged.Value;
        Assert.True(test.Age == 0);
    }

    [Fact]
    public void ChangeValueOfUnmanagedObject_WithAllocationManager()
    {
        Settings.UseAllocationManager = true;
        var unmanaged = new UnmanagedObject<Person>();
        var newPerson = Person.Create();
        unmanaged.Value = newPerson;
        Person testPerson = unmanaged.Value;
        Assert.True(testPerson.Age == 20);
    }

    [Fact]
    public void UpdateValueFromRefValue_WithAllocationManager()
    {
        Settings.UseAllocationManager = true;
        var unmanaged = new UnmanagedObject<Person>();
        var newPerson = Person.Create();
        unmanaged.Update(ref newPerson);
        Person testPerson = unmanaged.Value;
        Assert.True(testPerson.Age == 20);
    }

    [Fact]
    public void TestAllocationManager()
    {
        Settings.UseAllocationManager = true;
        Settings.MaxAllocations = 3;

        var unmanaged = new UnmanagedObject<Person>();
        var unmanaged2 = new UnmanagedObject<Person>();
        var unmanaged3 = new UnmanagedObject<Person>();
        var unmanaged4 = new UnmanagedObject<Person>();

        unmanaged.Dispose();
        unmanaged2.Dispose();
        unmanaged3.Dispose();
        unmanaged4.Dispose();

        Assert.True(
            AllocationManager.DisposedObjects.Count == 3 &&
            AllocationManager.Blocks.Count == 3 &&
            AllocationManager.Started == true);
    }
}