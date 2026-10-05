using System;
using Microsoft.Win32.SafeHandles;
namespace Soenneker.OpenZl.Util.Native;

internal sealed unsafe class FrameInfoHandle : SafeHandleZeroOrMinusOneIsInvalid
{
    public FrameInfoHandle(ReadOnlySpan<byte> bytes) : base(true)
    {
        fixed (byte* p = bytes) SetHandle(NativeMethods.ZL_FrameInfo_create(p, (nuint)bytes.Length));
        if (IsInvalid) throw new OpenZlException(10, "Invalid OpenZL frame header.");
    }
    protected override bool ReleaseHandle() { NativeMethods.ZL_FrameInfo_free(handle); return true; }
}
