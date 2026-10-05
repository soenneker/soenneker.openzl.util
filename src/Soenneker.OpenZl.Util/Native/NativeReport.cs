using System.Runtime.InteropServices;
namespace Soenneker.OpenZl.Util.Native;
// ZL_Report: result union for size_t. Both supported platforms use a 64-bit ABI.
[StructLayout(LayoutKind.Explicit, Size = 16)]
internal struct NativeReport
{
    [FieldOffset(0)] public int Code;
    [FieldOffset(8)] public nuint Value;
    [FieldOffset(8)] public nint ErrorInfo;
}
