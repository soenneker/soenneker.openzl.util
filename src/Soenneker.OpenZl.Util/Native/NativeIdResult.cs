using System.Runtime.InteropServices;
namespace Soenneker.OpenZl.Util.Native;
// Result<NodeID> stores its uint value at offset 4; the error union is 16 bytes.
[StructLayout(LayoutKind.Explicit, Size = 16)]
internal struct NativeIdResult
{
    [FieldOffset(0)] public int Code;
    [FieldOffset(4)] public uint Value;
    [FieldOffset(8)] public nint ErrorInfo;
}
