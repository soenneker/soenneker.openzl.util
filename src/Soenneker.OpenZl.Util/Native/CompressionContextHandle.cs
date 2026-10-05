using System;
using Microsoft.Win32.SafeHandles;
namespace Soenneker.OpenZl.Util.Native;

internal sealed class CompressionContextHandle : SafeHandleZeroOrMinusOneIsInvalid
{
    public CompressionContextHandle() : base(true)
    {
        SetHandle(NativeMethods.ZL_CCtx_create());
        if (IsInvalid) throw new OutOfMemoryException("Native OpenZL allocation failed.");
    }
    protected override bool ReleaseHandle() { NativeMethods.ZL_CCtx_free(handle); return true; }
}
