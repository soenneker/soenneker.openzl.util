using System;
using Microsoft.Win32.SafeHandles;

namespace Soenneker.OpenZl.Util.Native;

internal sealed class DecompressionContextHandle : SafeHandleZeroOrMinusOneIsInvalid
{
    public DecompressionContextHandle() : base(true)
    {
        SetHandle(NativeMethods.ZL_DCtx_create());
        if (IsInvalid)
            throw new OutOfMemoryException("Native OpenZL allocation failed.");
    }

    protected override bool ReleaseHandle()
    {
        NativeMethods.ZL_DCtx_free(handle);
        return true;
    }
}