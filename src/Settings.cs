namespace DenevCloud.Core.Unmanaged;

public static class Settings
{
    /// <summary>
    /// If true it will create memory block allocations on the heap and reuse them if the object in it is disposed. Recommended for higher performance. Default is false.
    /// </summary>
    public static bool UseAllocationManager { get; set; } = false;

    /// <summary>
    /// Maximum number of allocated memory blocks on the heap for storing objects. Default is 16.
    /// </summary>
    public static long MaxAllocations { get; set; } = 16;

    /// <summary>
    /// Maximum lifetime of an allocation block. If it passes the TimeSpan threshold the memory block is freed. Default is 2 minutes.
    /// </summary>
    public static TimeSpan MaxAllocationLifetime { get; set; } = TimeSpan.FromMinutes(2);

    /// <summary>
    /// The TimeSpan between every check for expired objects. Higher the value the more expired memory blocks might exist, lower the value and more checks will be made.
    /// </summary>
    public static TimeSpan ExpiredCheck { get; set; } = TimeSpan.FromSeconds(5);
}