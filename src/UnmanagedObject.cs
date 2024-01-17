using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace DenevCloud.Core.Unmanaged;

#nullable disable

#pragma warning disable CS8500 // This takes the address of, gets the size of, or declares a pointer to a managed type

[Serializable]
[StructLayout(LayoutKind.Sequential)]
public unsafe struct UnmanagedObject<T> : IDisposable where T : struct
{
    #region Parameters

    public int H_Size 
    { 
        get { return Unsafe.SizeOf<T>(); }
    }

    public IntPtr Handle { get; private set; }
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

    #endregion

    #region Constructors

    public UnmanagedObject()
    {
        AllocateNativeDefault();
    }

    public UnmanagedObject(void* pointer)
    {
        Handle = new(pointer);
    }

    public UnmanagedObject(T* value)
    {
        Handle = new IntPtr(value);
    }

    #endregion

    public T* GetHandle()
    {
        return (T*)Handle;
    }

    #region Updates

    public void Update(T Value)
    {
        if (!Disposed)
            Unsafe.Write((void*)Handle, Value);
        else
            throw new ObjectDisposedException($"UnmanagedObject<T{nameof(T)}> is disposed and cannot be updated.");
    }

    public void Update(T* Value)
    {
        if (!Disposed)
            Unsafe.Copy((void*)Handle, ref Unsafe.AsRef<T>(Value));
        else
            throw new ObjectDisposedException($"UnmanagedObject<T{nameof(T)}> is disposed and cannot be updated.");
    }

    public void Update(ref T Value)
    {
        if (!Disposed)
            Unsafe.Copy((void*)Handle, ref Value);
        else
            throw new ObjectDisposedException($"UnmanagedObject<T{nameof(T)}> is disposed and cannot be updated.");
    }

    #endregion

    public void Dispose()
    {
        Destroy();
    }

    #region Alloc / Dealloc

    internal void AllocateNativeDefault()
    {
        void* _pointer;

        if (Settings.UseAllocationManager)
            _pointer = (void*)AllocationManager.Allocate((nuint)H_Size);
        else
            _pointer = NativeMemory.Alloc((nuint)H_Size);

        T* pointer = (T*)_pointer;
        *pointer = default(T);
        Handle = new(pointer);
    }

    internal void Destroy()
    {
        if(Disposed) 
            return; 

        Disposed = true;

        if (Settings.UseAllocationManager)
        {
            var allowDispose = AllocationManager.Dispose(Handle);

            if (allowDispose)
                Handle = IntPtr.Zero;
        }
        else
        {
            NativeMemory.Free((void*)Handle);
            Handle = IntPtr.Zero;
        }
    }

    #endregion

    #region  Operators

    public static implicit operator T(UnmanagedObject<T> value)
    {
        if(value.Disposed)
            throw new ObjectDisposedException($"UnmanagedObject<T{nameof(T)}> is disposed and cannot be updated.");

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

    #endregion
}