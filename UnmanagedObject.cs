using System.Runtime.InteropServices;

namespace DenevCloud.Core.Unmanaged;

#nullable disable

#pragma warning disable CS8500 // This takes the address of, gets the size of, or declares a pointer to a managed type

[Serializable]
[StructLayout(LayoutKind.Sequential)]
public unsafe struct UnmanagedObject<T> : IDisposable, IAsyncDisposable where T : struct
{
    public int H_Size 
    { 
        get { return Marshal.SizeOf<T>(); }
    }

    public IntPtr Handle { get; private set; }
    public T Value { get { return ConvertPointerToStruct(Handle); } }
    public bool UseNative {  get; private set; }

    public UnmanagedObject()
    {
        UseNative = false;
        AllocateHGlobal();
    }

    public UnmanagedObject(nuint? size = null, bool useNative = false)
    {
        UseNative = useNative;

        if (useNative)
            AllocateNative(size);
        else
            AllocateHGlobal(size);
    }

    public UnmanagedObject(ref T? value, nuint? size = null, bool useNative = false)
    {
        UseNative = useNative;

        if (value is null)
            throw new ArgumentNullException(nameof(value));

        if(useNative)
            AllocateNative((T)value, size);
        else
            AllocateHGlobal((T)value, size);

        value = null;
    }

    public UnmanagedObject(T* value, nuint? size = null, bool useNative = false)
    {
        UseNative = useNative;

        if (useNative)
            AllocateNative(value, size);
        else
            AllocateHGlobal(value, size);
    }

    public void Dispose()
    {
        Destroy();
    }

    public ValueTask DisposeAsync()
    {
        return DestroyAsync();
    }

    internal void Destroy()
    {
        if(UseNative)
            NativeMemory.Free((void*)Handle);
        else
            Marshal.FreeHGlobal(Handle);

        Handle = IntPtr.Zero;
    }

    internal ValueTask DestroyAsync()
    {
        if (UseNative)
            NativeMemory.Free((void*)Handle);
        else
            Marshal.FreeHGlobal(Handle);

        Handle = IntPtr.Zero;
        return ValueTask.CompletedTask;
    }

    internal void AllocateNative(nuint? size = null)
    {
        if (size is null)
            Handle = new(NativeMemory.Alloc((nuint)H_Size));
        else
            Handle = new(NativeMemory.Alloc((nuint)size));

        T Value;
        T* Pointer = &Value;
        *Pointer = default(T);

        Marshal.StructureToPtr(*Pointer, Handle, true);
    }

    internal void AllocateHGlobal(nuint? size = null)
    {
        if (size is null)
            Handle = new(Marshal.AllocHGlobal(H_Size));
        else
            Handle = new(Marshal.AllocHGlobal((int)size));

        T Value;
        T* Pointer = &Value;
        *Pointer = default(T);

        Marshal.StructureToPtr(*Pointer, Handle, true);
    }

    internal void AllocateNative(T value, nuint? size = null)
    {
        if (size is null)
            Handle = new(NativeMemory.Alloc((nuint)H_Size));
        else
            Handle = new(NativeMemory.Alloc((nuint)size));

        Marshal.StructureToPtr(value, Handle, true);
    }

    internal void AllocateHGlobal(T value, nuint? size = null)
    {
        if (size is null)
            Handle = new(Marshal.AllocHGlobal(H_Size));
        else
            Handle = new(Marshal.AllocHGlobal((int)size));

        Marshal.StructureToPtr(value, Handle, true);
    }

    internal void AllocateNative(T* value, nuint? size = null)
    {
        if(size is null)
            Handle = new(NativeMemory.Alloc((nuint)H_Size));
        else
            Handle = new(NativeMemory.Alloc((nuint)size));

        Marshal.StructureToPtr(*value, Handle, true);
    }

    internal void AllocateHGlobal(T* value, nuint? size = null)
    {
        if (size is null)
            Handle = new(Marshal.AllocHGlobal(H_Size));
        else
            Handle = new(Marshal.AllocHGlobal((int)size));

        Marshal.StructureToPtr(*value, Handle, true);
    }

    internal static T ConvertPointerToStruct(IntPtr ptr)
    {
        return Marshal.PtrToStructure<T>(ptr);
    }

    internal static void StoreStructure(IntPtr ptr, T data)
    {
        Marshal.StructureToPtr(data, ptr, false);
    }

    public static implicit operator T(UnmanagedObject<T> value)
    {
        return ConvertPointerToStruct(value.Handle);
    }

    public static implicit operator UnmanagedObject<T>(T? data)
    {
        return new UnmanagedObject<T>(ref data);
    }

    public static implicit operator UnmanagedObject<T>(T* data)
    {
        return new UnmanagedObject<T>(data);
    }
}