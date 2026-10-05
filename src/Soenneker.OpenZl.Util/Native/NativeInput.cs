using System;
using System.Buffers;
namespace Soenneker.OpenZl.Util.Native;

internal sealed unsafe class NativeInput : IDisposable
{
    private MemoryHandle _bytes;
    private MemoryHandle _lengths;
    internal nint Handle { get; private set; }
    internal NativeInput(OpenZlData data)
    {
        try
        {
            _bytes = data.Bytes.Pin(); _lengths = data.StringLengths.Pin();
            Handle = data.Type switch
            {
                OpenZlDataType.Serial => NativeMethods.ZL_TypedRef_createSerial(_bytes.Pointer, (nuint)data.Bytes.Length),
                OpenZlDataType.Struct => NativeMethods.ZL_TypedRef_createStruct(_bytes.Pointer, (nuint)data.ElementWidth, (nuint)data.Count),
                OpenZlDataType.Numeric => NativeMethods.ZL_TypedRef_createNumeric(_bytes.Pointer, (nuint)data.ElementWidth, (nuint)data.Count),
                OpenZlDataType.String => NativeMethods.ZL_TypedRef_createString(_bytes.Pointer, (nuint)data.Bytes.Length, (uint*)_lengths.Pointer, (nuint)data.Count),
                _ => throw new ArgumentOutOfRangeException(nameof(data))
            };
            if (Handle == 0) throw new OutOfMemoryException("Native typed input allocation failed.");
        }
        catch { Dispose(); throw; }
    }
    public void Dispose()
    {
        if (Handle != 0) { NativeMethods.ZL_TypedRef_free(Handle); Handle = 0; }
        _lengths.Dispose(); _lengths = default; _bytes.Dispose(); _bytes = default;
    }
}
