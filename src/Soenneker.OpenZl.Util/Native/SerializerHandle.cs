using System;
using Microsoft.Win32.SafeHandles;
namespace Soenneker.OpenZl.Util.Native;

internal sealed class SerializerHandle : SafeHandleZeroOrMinusOneIsInvalid
{
    public SerializerHandle() : base(true)
    {
        SetHandle(NativeMethods.ZL_CompressorSerializer_create());
        if (IsInvalid) throw new OutOfMemoryException("Native OpenZL allocation failed.");
    }
    protected override bool ReleaseHandle() { NativeMethods.ZL_CompressorSerializer_free(handle); return true; }
}
