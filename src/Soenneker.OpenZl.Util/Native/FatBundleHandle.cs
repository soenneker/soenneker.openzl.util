using System;
using Microsoft.Win32.SafeHandles;
namespace Soenneker.OpenZl.Util.Native;

internal sealed class FatBundleHandle : SafeHandleZeroOrMinusOneIsInvalid
{
    internal FatBundleHandle() : base(true)
    {
        SetHandle(NativeMethods.ZL_FatBundleDictLoader_create());
        if (IsInvalid) throw new OutOfMemoryException();
    }
    protected override bool ReleaseHandle() { NativeMethods.ZL_FatBundleDictLoader_free(handle); return true; }
}
