using System;
using Microsoft.Win32.SafeHandles;
namespace Soenneker.OpenZl.Util.Native;

internal sealed class CompressorHandle : SafeHandleZeroOrMinusOneIsInvalid
{
    public CompressorHandle() : base(true)
    {
        SetHandle(NativeMethods.ZL_Compressor_create());
        if (IsInvalid) throw new OutOfMemoryException("Native OpenZL allocation failed.");
    }
    protected override bool ReleaseHandle() { NativeMethods.ZL_Compressor_free(handle); return true; }
}
