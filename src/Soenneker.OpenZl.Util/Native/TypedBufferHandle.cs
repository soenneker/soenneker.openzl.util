using System;
using Microsoft.Win32.SafeHandles;
namespace Soenneker.OpenZl.Util.Native;

internal sealed class TypedBufferHandle : SafeHandleZeroOrMinusOneIsInvalid
{
    public TypedBufferHandle() : base(true)
    {
        SetHandle(NativeMethods.ZL_TypedBuffer_create());
        if (IsInvalid) throw new OutOfMemoryException("Native OpenZL allocation failed.");
    }
    protected override bool ReleaseHandle() { NativeMethods.ZL_TypedBuffer_free(handle); return true; }
}
