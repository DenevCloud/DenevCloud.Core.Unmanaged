using Microsoft.Extensions.Primitives;

namespace DenevCloud.Core.Unmanaged;

public struct Person
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

    public Guid Id { get; set; }
    public StringValues Name { get; set; }
    public byte Age { get; set; }
}
