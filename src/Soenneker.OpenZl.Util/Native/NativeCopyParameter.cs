using System.Runtime.InteropServices;
namespace Soenneker.OpenZl.Util.Native;

[StructLayout(LayoutKind.Sequential)]
internal unsafe struct NativeCopyParameter
{
    internal int Id; internal void* Data; internal nuint Size;
}
