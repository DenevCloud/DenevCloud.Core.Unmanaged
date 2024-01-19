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

    private IntPtr Handle { get; set; }

    public bool Disposed { get; private set; }

    public ref T RefValueUnsafe
    { 
        get 
        {
            if (Disposed)
                throw new ObjectDisposedException($"UnmanagedObject<{nameof(T)}> is disposed and the value is no longer available");

            return ref Unsafe.AsRef<T>((void*)Handle); 
        } 
    }

    public T Value
    {
        get
        {
            if (Disposed)
                throw new ObjectDisposedException($"UnmanagedObject<{nameof(T)}> is disposed and the value is no longer available");

            return Unsafe.AsRef<T>((void*)Handle);
        }

        set
        {
            if (Disposed)
                throw new ObjectDisposedException($"UnmanagedObject<{nameof(T)}> is disposed and the value is no longer available");

            Unsafe.Write((void*)Handle, value);
        }
    }

    #endregion

    #region Constructors

    /// <summary>
    /// Create an UnmanagedObject<T> with default(T) value.
    /// </summary>
    public UnmanagedObject()
    {
        AllocateNativeDefault();
    }

    /// <summary>
    /// Create an UnmanagedObject<T> from a pointer. It must be from unmanaged object.
    /// </summary>
    /// <param name="pointer">A pointer to the object of an unknown type.</param>
    public UnmanagedObject(void* pointer)
    {
        Handle = new(pointer);
    }

    /// <summary>
    /// Create an UnmanagedObject<T> from a pointer. It must be from unmanaged object.
    /// </summary>
    /// <param name="value">A pointer to the object of a T type.</param>
    public UnmanagedObject(T* value)
    {
        Handle = new IntPtr(value);
    }

    #endregion

    /// <summary>
    /// Returns a pointer of a T type, which points to the underlying object.
    /// </summary>
    /// <returns>Returns a pointer of a T type, which points to the underlying object.</returns>
    /// <exception cref="ObjectDisposedException">If the UnmanagedObject<T> is disposed then it throws an exception.</exception>
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
            throw new ObjectDisposedException($"UnmanagedObject<{nameof(T)}> is disposed and cannot be updated.");
    }

    public void Update(T* Value)
    {
        if (!Disposed)
            Unsafe.Copy((void*)Handle, ref Unsafe.AsRef<T>(Value));
        else
            throw new ObjectDisposedException($"UnmanagedObject<{nameof(T)}> is disposed and cannot be updated.");
    }

    public void Update(ref T Value)
    {
        if (!Disposed)
            Unsafe.Copy((void*)Handle, ref Value);
        else
            throw new ObjectDisposedException($"UnmanagedObject<{nameof(T)}> is disposed and cannot be updated.");
    }

    #endregion

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

    /// <summary>
    /// Frees the memory block if Settings.UseAllocationManager is false otherwise, clears the memory block for reuse.
    /// </summary>
    public void Dispose()
    {
        if (Disposed)
            return;

        if (Settings.UseAllocationManager)
        {
            var allowDispose = AllocationManager.Dispose(Handle, (nuint)H_Size);

            if (allowDispose)
                Handle = IntPtr.Zero;
        }
        else
        {
            NativeMemory.Free((void*)Handle);
            Handle = IntPtr.Zero;
            Disposed = true;
        }

        GC.SuppressFinalize(this);
    }

    #endregion

    #region  Operators

    public static implicit operator T(UnmanagedObject<T> value)
    {
        if(value.Disposed)
            throw new ObjectDisposedException($"UnmanagedObject<T{nameof(T)}> is disposed and cannot be updated.");

        return Unsafe.Read<T>(value.GetHandle());
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