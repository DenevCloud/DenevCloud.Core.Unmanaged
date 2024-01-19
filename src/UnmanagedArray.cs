using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace DenevCloud.Core.Unmanaged;

#pragma warning disable CS8500 // This takes the address of, gets the size of, or declares a pointer to a managed type
#pragma warning disable CS0693 // Type parameter has the same name as the type parameter from outer type

[Serializable]
[StructLayout(LayoutKind.Sequential)]
public unsafe struct UnmanagedArray<T> : IDisposable where T : struct
{
    #region Parameters

    public int H_Size
    {
        get { return Unsafe.SizeOf<T>(); }
    }

    public int TotalH_Size
    {
        get { return _array_size * Unsafe.SizeOf<T>(); }
    }

    private int _array_size;

    public int Array_Size
    {
        get { return _array_size; }
        internal set { _array_size = value; }
    }

    private IntPtr Handle { get; set; }

    public bool Disposed { get; private set; }

    public T this[int index]
    {
        get
        {
            if (Disposed)
                throw new ObjectDisposedException($"UnmanagedArray<{nameof(T)}> is disposed and the value is no longer available");

            if (index > Array_Size)
                throw new IndexOutOfRangeException();

            return Unsafe.AsRef<T>((void*)(Handle + index * H_Size));
        }

        set
        {
            if (Disposed)
                throw new ObjectDisposedException($"UnmanagedArray<{nameof(T)}> is disposed and the value is no longer available");

            if (index > Array_Size)
                throw new IndexOutOfRangeException();

            Unsafe.Write((void*)(Handle + index * H_Size), value);
        }
    }

    public T[] Array
    {
        get
        {
            if (Disposed)
                throw new ObjectDisposedException($"UnmanagedArray<{nameof(T)}> is disposed and the value is no longer available");

            var _array = new T[Array_Size];

            for (int i = 0; i < Array_Size; i++)
            {
                _array[i] = Unsafe.AsRef<T>((void*)(Handle + i * H_Size));
            }

            return _array;
        }
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

    #region Constructors

    /// <summary>
    /// Creates an unmanaged array with capacity of 1 and value default(T).
    /// </summary>
    public UnmanagedArray()
    {
        Array_Size = 1;
        AllocateNativeDefault();
    }

    /// <summary>
    /// Creates an unmanaged array with capasity of 'size' and value default(T).
    /// </summary>
    public UnmanagedArray(int size)
    {
        Array_Size = size;
        AllocateNativeDefault();
    }

    #endregion

    #region Alloc / Dealloc

    internal void AllocateNativeDefault()
    {
        void* _pointer = NativeMemory.Alloc((nuint)(TotalH_Size));

        Handle = new(_pointer);

        for (int i = 0; i < TotalH_Size; i = i + H_Size)
        {
            var loc = (void*)(Handle + i);
            Unsafe.Write(loc, default(T)); 
        }
    }

    public void Dispose()
    {
        if (Disposed)
            return;

        NativeMemory.Free((void*)Handle);
        Handle = IntPtr.Zero;
        Disposed = true;

        GC.SuppressFinalize(this);
    }

    #endregion

    #region  Operators

    public static implicit operator T[](UnmanagedArray<T> value)
    {
        if (value.Disposed)
            throw new ObjectDisposedException($"UnmanagedArray<T{nameof(T)}> is disposed and cannot be updated.");

        var _array = new T[value.Array_Size];

        for (int i = 0; i < value.Array_Size; i++)
        {
            _array[i] = Unsafe.AsRef<T>((value.GetHandle() + i * value.H_Size));
        }

        return _array;
    }

    public static implicit operator UnmanagedArray<T>(T[] data)
    {
        var unmanaged = new UnmanagedArray<T>(data.Length);

        for(int i = 0; i < data.Length; i++)
        {
            unmanaged[i] = data[i];
        }

        return unmanaged;
    }

    #endregion
}
