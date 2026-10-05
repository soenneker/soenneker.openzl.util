using System.Runtime.InteropServices;
namespace Soenneker.OpenZl.Util.Native;

[StructLayout(LayoutKind.Sequential)]
internal unsafe struct NativeLocalParameters
{
    internal NativeIntParameter* Integers; internal nuint IntegerCount; internal NativeCopyParameter* Copies; internal nuint CopyCount; internal void* References; internal nuint ReferenceCount;
}
