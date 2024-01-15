using System.Runtime.InteropServices;

namespace DenevCloud.Core.Unmanaged;

#pragma warning disable CS8500 // This takes the address of, gets the size of, or declares a pointer to a managed type
#nullable disable

[Serializable]
[StructLayout(LayoutKind.Sequential)]
internal unsafe struct UnmanagedObject_Old<T> : IDisposable, IAsyncDisposable where T : struct
{
    private IntPtr _Handle;
    private bool _IsEmpty;
    private bool _IsDisposed;

    public UnmanagedObject_Old(ref T obj)
    {
        _Handle = Marshal.AllocHGlobal(Marshal.SizeOf<T>());
        Marshal.StructureToPtr(obj, _Handle, false);
        _IsEmpty = false;
        _IsDisposed = false;
    }

    public UnmanagedObject_Old(T* value)
    {
        _Handle = Marshal.AllocHGlobal(Marshal.SizeOf<T>());
        Marshal.StructureToPtr<T>(*value, _Handle, false);
        _IsEmpty = false;
        _IsDisposed = false;
    }

    public UnmanagedObject_Old() 
    { 
        _Handle = IntPtr.Zero;
        _IsEmpty = true;
    }

    public T Value { get { return (!_IsEmpty) ? Marshal.PtrToStructure<T>(_Handle) : default; } }

    public bool IsEmpty { get { return _IsEmpty; } }

    public bool IsDisposed { get { return _IsDisposed; } }

    public void Dispose()
    {
        Destroy();
    }

    public ValueTask DisposeAsync()
    {
        return DestroyAsync();
    }

    public void SetValue(ref T obj)
    {
        _Handle = Marshal.AllocHGlobal(Marshal.SizeOf<T>());
        Marshal.StructureToPtr(obj, _Handle, false);
        _IsEmpty = false;
        _IsDisposed = false;
    }

    public void SetValue(T* value)
    {
        _Handle = Marshal.AllocHGlobal(Marshal.SizeOf<T>());
        Marshal.StructureToPtr<T>(*value, _Handle, false);
        _IsEmpty = false;
        _IsDisposed = false;
    }

    private void Destroy()
    {
        if(!_IsEmpty && !_IsDisposed)
        {
            Marshal.FreeHGlobal(_Handle);
            _Handle = IntPtr.Zero;
            _IsDisposed = true;
            _IsEmpty = true;
        }

        GC.SuppressFinalize(this);
    }

    private ValueTask DestroyAsync()
    {
        if (!_IsEmpty && !_IsDisposed)
        {
            Marshal.FreeHGlobal(_Handle);
            _Handle = IntPtr.Zero;
            _IsDisposed = true;
            _IsEmpty = true;
        }

        GC.SuppressFinalize(this);

        return ValueTask.CompletedTask;
    }

    private byte[] GetBytes(T* value)
    {
        int size = Marshal.SizeOf<T>();
        byte[] arr = new byte[size];

        IntPtr ptr = IntPtr.Zero;

        try
        {
            ptr = Marshal.AllocHGlobal(size);
            Marshal.StructureToPtr(*value, ptr, true);
            Marshal.Copy(ptr, arr, 0, size);
        }
        finally
        {
            Marshal.FreeHGlobal(ptr);
        }
        return arr;
    }

    private byte[] GetBytes(T value)
    {
        int size = Marshal.SizeOf<T>();
        byte[] arr = new byte[size];

        IntPtr ptr = IntPtr.Zero;

        try
        {
            ptr = Marshal.AllocHGlobal(size);
            Marshal.StructureToPtr(value, ptr, true);
            Marshal.Copy(ptr, arr, 0, size);
        }
        finally
        {
            Marshal.FreeHGlobal(ptr);
        }

        return arr;
    }

    private T FromBytes(ref byte[] arr)
    {
        T val;

        int size = Marshal.SizeOf<T>();

        IntPtr ptr = IntPtr.Zero;

        try
        {
            ptr = Marshal.AllocHGlobal(size);

            Marshal.Copy(arr, 0, ptr, size);

            val = (T)Marshal.PtrToStructure(ptr, typeof(T));
        }
        finally
        {
            Marshal.FreeHGlobal(ptr);
        }

        return val;
    }
}