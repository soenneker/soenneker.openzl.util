using System.Runtime.InteropServices;
namespace Soenneker.OpenZl.Util.Native;

[StructLayout(LayoutKind.Sequential)]
internal unsafe struct NativeNodeParameters
{
    internal byte* Name; internal NativeLocalParameters* Local; internal fixed byte DictionaryId[32]; internal void* MaterializedData; internal nuint MaterializedSize; internal fixed byte MaterializedId[32];
}
