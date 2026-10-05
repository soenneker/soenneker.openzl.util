using System.Runtime.InteropServices;
namespace Soenneker.OpenZl.Util.Native;

[StructLayout(LayoutKind.Sequential)]
internal unsafe struct NativeIntParameter
{
    internal int Id; internal int Value;
}
