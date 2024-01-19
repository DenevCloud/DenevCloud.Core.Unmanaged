namespace DenevCloud.Core.Unmanaged;

public struct AllocatedMemoryBlock
{
    internal DateTime Expires { get; set; }
    internal IntPtr Id { get; set; }
    internal nuint Size { get; set; }
}