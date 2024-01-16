using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace DenevCloud.Core.Unmanaged;

#nullable disable

#pragma warning disable CS8500 // This takes the address of, gets the size of, or declares a pointer to a managed type

[Serializable]
[StructLayout(LayoutKind.Sequential)]
public unsafe struct UnmanagedObject<T> : IDisposable where T : struct
{
    public int H_Size 
    { 
        get { return Marshal.SizeOf<T>(); }
    }

    public IntPtr Handle { get; private set; }
    public bool UseNative {  get; private set; }
    public bool Disposed { get; private set; }

    public ref T RefValue 
    { 
        get 
        {
            if (Disposed)
                throw new ObjectDisposedException("UnmanagedObject<T> is disposed and the value is no longer available");

            return ref Unsafe.AsRef<T>((void*)Handle); 
        } 
    }

    public T Value
    {
        get
        {
            if (Disposed)
                throw new ObjectDisposedException("UnmanagedObject<T> is disposed and the value is no longer available");

            return Unsafe.AsRef<T>((void*)Handle);
        }

        set
        {
            Unsafe.Write((void*)Handle, value);
        }
    }

    public UnmanagedObject()
    {
        UseNative = false;
        AllocateUnsafe();
    }

    public UnmanagedObject(void* pointer)
    {
        Handle = new(pointer);
    }

    public UnmanagedObject(nuint? size = null, bool useNative = false)
    {
        UseNative = useNative;

        if (useNative)
            AllocateNative(size);
        else
            AllocateHGlobal(size);
    }

    public UnmanagedObject(T* value, nuint? size = null, bool useNative = false)
    {
        Handle = new IntPtr(value);
    }

    public void Dispose()
    {
        Destroy();
    }

    public void Update(T Value)
    {
        Unsafe.Write((void*)Handle, Value);
    }

    public void Update(T* Value)
    {
        Unsafe.Copy((void*)Handle, ref Unsafe.AsRef<T>(Value));
    }

    public void Update(ref T Value)
    {
        Unsafe.Copy((void*)Handle, ref Value);
    }

    internal void Destroy()
    {
        if(Disposed) 
            return; 

        Disposed = true;

        if(UseNative)
            NativeMemory.Free((void*)Handle);
        else
            Marshal.FreeHGlobal(Handle);

        Handle = IntPtr.Zero;
    }

    public T* GetHandle()
    {
        return (T*)Handle;
    }

    internal void AllocateUnsafe()
    {
        var _pointer = Marshal.AllocHGlobal(H_Size);
        T* objectPointer = (T*)_pointer;
        *objectPointer = default(T);
        Handle = new(objectPointer);
    }

    internal void AllocateNative(nuint? size = null)
    {
        if (size is null)
            Handle = new(NativeMemory.Alloc((nuint)H_Size));
        else
            Handle = new(NativeMemory.Alloc((nuint)size));

        T* objectPointer = (T*)Handle;
        *objectPointer = default(T);
    }

    internal void AllocateHGlobal(nuint? size = null)
    {
        if (size is null)
            Handle = new(Marshal.AllocHGlobal(H_Size));
        else
            Handle = new(Marshal.AllocHGlobal((int)size));

        T* objectPointer = (T*)Handle;
        *objectPointer = default(T);
    }

    public static implicit operator T(UnmanagedObject<T> value)
    {
        return Unsafe.Read<T>((void*)value.Handle);
    }

    public static implicit operator UnmanagedObject<T>(T* data)
    {
        return new UnmanagedObject<T>(data);
    }

    public static implicit operator UnmanagedObject<T>(T data)
    {
        var pointer = Unsafe.AsPointer<T>(ref data);
        return new UnmanagedObject<T>(pointer);
    }
}