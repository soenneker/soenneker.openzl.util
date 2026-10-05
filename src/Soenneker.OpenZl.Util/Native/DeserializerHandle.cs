using System;
using Microsoft.Win32.SafeHandles;
namespace Soenneker.OpenZl.Util.Native;

internal sealed class DeserializerHandle : SafeHandleZeroOrMinusOneIsInvalid
{
    public DeserializerHandle() : base(true)
    {
        SetHandle(NativeMethods.ZL_CompressorDeserializer_create());
        if (IsInvalid) throw new OutOfMemoryException("Native OpenZL allocation failed.");
    }
    protected override bool ReleaseHandle() { NativeMethods.ZL_CompressorDeserializer_free(handle); return true; }
}
