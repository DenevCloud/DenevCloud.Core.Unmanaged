using Microsoft.Extensions.Primitives;

namespace DenevCloud.Core.Unmanaged.Tests;

#pragma warning disable CS8500

public unsafe partial class UnmanagedObjectTest
{
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
        internal byte Age;
    }
}