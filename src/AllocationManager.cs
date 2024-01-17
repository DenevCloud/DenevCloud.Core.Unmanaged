using System.Collections.Concurrent;
using System.Collections.Immutable;
using System.Runtime.InteropServices;

namespace DenevCloud.Core.Unmanaged;

#nullable disable

public static class AllocationManager
{
    public static List<IntPtr> DisposedObjects { get; private set; } = new List<IntPtr>();
    public static ConcurrentStack<AllocatedMemoryBlock> Blocks { get; private set; } = new ConcurrentStack<AllocatedMemoryBlock>();

    public static bool Started { get; private set; } = false;

    internal static Action CreateCheckExpiredAction = new Action(async () =>
    {
        while (Settings.UseAllocationManager)
        {
            Started = true;
            CleanExpired();
            await Task.Delay(Settings.ExpiredCheck);
        }

        Started = false;
    });

    internal static Task CheckExpired = Task.Factory.StartNew(CreateCheckExpiredAction);

    public unsafe static void CleanExpired()
    {
        Repeat:

        if (Blocks.TryPeek(out var block) && block.Expires <= DateTime.Now)
        {
            NativeMemory.Free((void*)block.Id);
            Blocks.TryPop(out _);
            goto Repeat;
        }
    }

    /// <summary>
    /// Clears all memory blocks and their respective values. After calling this method all 'UnmanagedObject<T>' will be unusable.
    /// </summary>
    public unsafe static void CleanMemory()
    {
        Repeat:

        if (Blocks.TryPeek(out var block))
        {
            NativeMemory.Free((void*)block.Id);
            Blocks.TryPop(out _);
            goto Repeat;
        }
    }

    public unsafe static IntPtr Allocate(nuint Size)
    {
        if (Settings.UseAllocationManager && !Started)
            CheckExpired = Task.Factory.StartNew(CreateCheckExpiredAction);

        foreach (var block in Blocks)
        {
            //Check for expired memory blocks which are disposed
            if (DisposedObjects.Contains(block.Id))
            {
                DisposedObjects.Remove(block.Id);

                NativeMemory.Clear((void*)block.Id, Size);

                return block.Id;
            }
        }

        //Alloc new memory block
        var _pointer = new IntPtr(NativeMemory.AllocZeroed(Size));

        //Push the newly created memory block
        Blocks.Push(new AllocatedMemoryBlock()
        {
            Size = Size,
            Expires = DateTime.Now + Settings.MaxAllocationLifetime,
            Id = _pointer
        });  

        return _pointer;
    }

    public unsafe static bool Dispose(IntPtr pointer)
    {
        if(Blocks.Count > Settings.MaxAllocations)
        {
            NativeMemory.Free((void*)pointer);

            var _array = Blocks.OrderByDescending(x => x.Expires).ToImmutableArray();

            Blocks.Clear();

            foreach(var item in _array)
            {
                if (!DisposedObjects.Contains(item.Id) && item.Expires > DateTime.Now && item.Id != pointer)
                    Blocks.Push(item);
            }

            return true;
        }
        else
        {
            DisposedObjects.Add(pointer);
            return false;
        }
    }
}

public struct AllocatedMemoryBlock
{
    internal DateTime Expires { get; set; }
    internal IntPtr Id { get; set; }
    internal nuint Size { get; set; }
}