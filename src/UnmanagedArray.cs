using Microsoft.Extensions.Primitives;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace DenevCloud.Core.Unmanaged;

#pragma warning disable CS8500 // This takes the address of, gets the size of, or declares a pointer to a managed type
#nullable disable

[Serializable]
[StructLayout(LayoutKind.Sequential)]
public unsafe struct UnmanagedArray<T> : IDisposable where T : struct
{
    #region Parameters

    public long H_Size
    {
        get { return Unsafe.SizeOf<T>(); }
    }

    public long TotalH_Size
    {
        get { return _length * H_Size; }
    }

    private long _length;

    public long Length
    {
        get { return _length; }
        internal set { _length = value; }
    }

    private IntPtr Handle { get; set; }

    public bool Disposed { get; private set; }

    public T this[int index]
    {
        get
        {
            if (Disposed)
                throw new ObjectDisposedException($"UnmanagedArray<{nameof(T)}> is disposed and the value is no longer available");

            if (index > Length)
                throw new IndexOutOfRangeException();

            return Unsafe.AsRef<T>((void*)(Handle + index * H_Size));
        }

        set
        {
            if (Disposed)
                throw new ObjectDisposedException($"UnmanagedArray<{nameof(T)}> is disposed and the value is no longer available");

            if (index > Length)
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

            var _array = new T[Length];

            for (int i = 0; i < Length; i++)
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

    public void SortBy<TType>(ref StringValues fieldName) where TType : unmanaged
    {
        BubbleSort<TType>(ref fieldName);
    }

    public void Resize(long newLength)
    {
        if (newLength <= _length)
            throw new InvalidOperationException("This method can be used only to enlarge the memory block that is occupied by the array of T.");

        Length = newLength;

        Handle = new IntPtr(NativeMemory.Realloc((void*)Handle, (nuint)TotalH_Size));
    }

    #region Constructors

    /// <summary>
    /// Creates an unmanaged array with capacity of 1 and value default(T).
    /// </summary>
    public UnmanagedArray()
    {
        Length = 1;
        AllocateNativeDefault();
    }

    /// <summary>
    /// Creates an unmanaged array with capasity of 'size' and value default(T).
    /// </summary>
    public UnmanagedArray(int size)
    {
        Length = size;
        AllocateNativeDefault();
    }

    #endregion

    #region Alloc / Dealloc

    internal void AllocateNativeDefault()
    {
        void* _pointer = NativeMemory.Alloc((nuint)(TotalH_Size));

        Handle = new(_pointer);

        for (long i = 0; i < TotalH_Size; i = i + H_Size)
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
    }

    #endregion

    #region  Operators

    public static implicit operator T[](UnmanagedArray<T> value)
    {
        if (value.Disposed)
            throw new ObjectDisposedException($"UnmanagedArray<T{nameof(T)}> is disposed and cannot be updated.");

        var _array = new T[value.Length];

        for (int i = 0; i < value.Length; i++)
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

    private void BubbleSort<TType>(ref StringValues fieldName) where TType : unmanaged
    {
        for (int i = 0; i < _length - 1; i++)
        {
            bool swapped = false;
            for (int j = 0; j < _length - i - 1; j++)
            {
                var value1 = (TType)typeof(T).GetProperty(fieldName).GetValue(this[j]);
                var value2 = (TType)typeof(T).GetProperty(fieldName).GetValue(this[j + 1]);

                int result = Comparer<TType>.Default.Compare(value1, value2);

                if (result > 0)
                {
                    var _temp = this[j + 1];
                    this[j + 1] = this[j];
                    this[j] = _temp;
                    swapped = true;
                }
            }
            if (!swapped)
            {
                break;
            }
        }
    }
}