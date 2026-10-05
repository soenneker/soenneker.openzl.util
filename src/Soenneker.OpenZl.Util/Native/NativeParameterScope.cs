using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Soenneker.OpenZl.Util.Options;
namespace Soenneker.OpenZl.Util.Native;

internal sealed unsafe class NativeParameterScope : IDisposable
{
    private readonly List<nint> _allocations = new();
    internal NativeLocalParameters* Pointer { get; }
    private void* Allocate(nuint size)
    {
        void* p = NativeMemory.AllocZeroed(size == 0 ? 1 : size);
        if (p == null) throw new OutOfMemoryException();
        _allocations.Add((nint)p); return p;
    }
    internal NativeParameterScope(OpenZlLocalParameters? parameters)
    {
        if (parameters == null) return;
        ArgumentNullException.ThrowIfNull(parameters.Integers); ArgumentNullException.ThrowIfNull(parameters.Data);
        try
        {
            Pointer = (NativeLocalParameters*)Allocate((nuint)sizeof(NativeLocalParameters));
            // Snapshot collections before calculating buffer sizes.
            var integers = new List<KeyValuePair<int, int>>(parameters.Integers);
            var copies = new List<KeyValuePair<int, ReadOnlyMemory<byte>>>(parameters.Data);
            Pointer->IntegerCount = (nuint)integers.Count; Pointer->CopyCount = (nuint)copies.Count;
            Pointer->Integers = (NativeIntParameter*)Allocate(checked((nuint)integers.Count * (nuint)sizeof(NativeIntParameter)));
            Pointer->Copies = (NativeCopyParameter*)Allocate(checked((nuint)copies.Count * (nuint)sizeof(NativeCopyParameter)));
            for (int i = 0; i < integers.Count; i++) Pointer->Integers[i] = new() { Id = integers[i].Key, Value = integers[i].Value };
            for (int i = 0; i < copies.Count; i++)
            {
                var data = copies[i].Value; void* buffer = Allocate((nuint)data.Length); data.Span.CopyTo(new Span<byte>(buffer, data.Length));
                Pointer->Copies[i] = new() { Id = copies[i].Key, Data = buffer, Size = (nuint)data.Length };
            }
        }
        catch { Dispose(); throw; }
    }
    public void Dispose() { foreach (nint p in _allocations) NativeMemory.Free((void*)p); _allocations.Clear(); }
}
