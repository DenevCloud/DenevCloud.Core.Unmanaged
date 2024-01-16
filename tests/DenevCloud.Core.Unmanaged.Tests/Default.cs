using Microsoft.Extensions.Primitives;

namespace DenevCloud.Core.Unmanaged.Tests;

#pragma warning disable CS8500

public unsafe class Default
{
    [Fact]
    public void InitAndReadWithPointer()
    {
        Person person = Person.Create();
        var _pointer = &person;
        var unmanaged = new UnmanagedObject<Person>(_pointer);

        Person testPerson = unmanaged;

        Assert.True(testPerson.Age == person.Age);
    }

    [Fact]
    public void InitAndReadEmpty()
    {
        var unmanaged = new UnmanagedObject<Person>();
        Assert.True(((Person)unmanaged).Age == 0);
    }

    [Fact]
    public void GetRefValue()
    {
        var unmanaged = new UnmanagedObject<Person>();

        ref var test = ref unmanaged.RefValue;

        Assert.True(test.Age == 0);
    }

    [Fact]
    public void Dispose()
    {
        var unmanaged = new UnmanagedObject<Person>();

        ref var test = ref unmanaged.RefValue;

        unmanaged.Dispose();

        Assert.True(unmanaged.Disposed == true && unmanaged.Handle == IntPtr.Zero);
    }


    [Fact]
    public void UnmanagedObjectToObject()
    {
        var unmanaged = new UnmanagedObject<Person>();
        var test =  unmanaged.Value;
        Assert.True(test.Age == 0);
    }

    [Fact]
    public void ChangeValueOfUnmanagedObject()
    {
        var unmanaged = new UnmanagedObject<Person>();
        var newPerson = Person.Create();
        unmanaged.Value = newPerson;

        Person testPerson = unmanaged.Value;

        Assert.True(testPerson.Age == 20);
    }

    [Fact]
    public void UpdateValueFromRefValue()
    {
        var unmanaged = new UnmanagedObject<Person>();
        var newPerson = Person.Create();
        unmanaged.Update(ref newPerson);

        Person testPerson = unmanaged.Value;

        Assert.True(testPerson.Age == 20);
    }

    internal struct Person
    {
        public static Person Create()
        {
            return new Person
            {
                Id = Guid.Empty,
                Name = new StringValues("Mylo"),
                Age = 20
            };
        }

        internal Guid Id { get; set; }
        internal StringValues Name { get; set; }
        internal byte Age { get; set; }
    }
}