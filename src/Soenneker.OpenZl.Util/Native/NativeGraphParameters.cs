using System.Runtime.InteropServices;
namespace Soenneker.OpenZl.Util.Native;

[StructLayout(LayoutKind.Sequential)]
internal unsafe struct NativeGraphParameters
{
    internal byte* Name; internal uint* Graphs; internal nuint GraphCount; internal uint* Nodes; internal nuint NodeCount; internal NativeLocalParameters* Local; internal void* MaterializedData; internal nuint MaterializedSize; internal fixed byte MaterializedId[32];
}
