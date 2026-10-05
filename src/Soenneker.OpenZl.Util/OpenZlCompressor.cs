using System;
using System.Collections.Generic;
using System.Text;
using Soenneker.OpenZl.Util.Abstract;
using Soenneker.OpenZl.Util.Native;
using Soenneker.OpenZl.Util.Options;
namespace Soenneker.OpenZl.Util;

public sealed unsafe class OpenZlCompressor : IOpenZlCompressor
{
    private readonly object _gate = new();
    private readonly CompressorHandle _handle = new();
    internal OpenZlCompressor(OpenZlGraph graph) { try { SelectGraph(GetGraph(graph)); } catch { _handle.Dispose(); throw; } }
    internal OpenZlCompressor(ReadOnlySpan<byte> data, ReadOnlySpan<byte> bundle)
    {
        try
        {
            using var deserializer = new DeserializerHandle();
            fixed (byte* p = data, b = bundle)
                NativeErrors.Check(NativeMethods.ZL_CompressorDeserializer_deserialize(deserializer, _handle, p, (nuint)data.Length, b, (nuint)bundle.Length), r => NativeMethods.ZL_CompressorDeserializer_getErrorContextString(deserializer, r));
        }
        catch { _handle.Dispose(); throw; }
    }
    private void Alive() => ObjectDisposedException.ThrowIf(_handle.IsClosed, this);
    private uint Id(OpenZlGraphReference graph)
    {
        ArgumentNullException.ThrowIfNull(graph);
        if (!ReferenceEquals(graph.Owner, this)) throw new ArgumentException("Graph belongs to another compressor.", nameof(graph));
        return graph.Id;
    }
    private uint Id(OpenZlNodeReference node)
    {
        ArgumentNullException.ThrowIfNull(node);
        if (!ReferenceEquals(node.Owner, this)) throw new ArgumentException("Node belongs to another compressor.", nameof(node));
        return node.Id;
    }
    private uint CheckId(NativeIdResult result)
    {
        if (result.Code != 0) NativeErrors.Check(new NativeReport { Code = result.Code, ErrorInfo = result.ErrorInfo }, r => NativeMethods.ZL_Compressor_getErrorContextString(_handle, r));
        return result.Value;
    }
    private static byte[]? Name(string? name)
    {
        if (name?.Contains('\0') == true) throw new ArgumentException("Name contains a null character.", nameof(name));
        return name == null ? null : Encoding.UTF8.GetBytes(name + "\0");
    }
    public OpenZlNodeReference GetNode(OpenZlNode node)
    {
        lock (_gate) { Alive(); if (!Enum.IsDefined(node) || (uint)node < 2) throw new ArgumentOutOfRangeException(nameof(node)); return new(this, (uint)node); }
    }
    public OpenZlNodeReference RegisterStructConversion(int width)
    {
        if (width < 1) throw new ArgumentOutOfRangeException(nameof(width));
        lock (_gate) { Alive(); return new(this, CheckId(NativeMethods.ZL_Compressor_parameterizeConvertSerialToStructNode(_handle, width))); }
    }
    public OpenZlNodeReference ParameterizeNode(OpenZlNodeReference node, OpenZlLocalParameters parameters, string? name = null)
    {
        ArgumentNullException.ThrowIfNull(parameters);
        lock (_gate)
        {
            Alive(); using var scope = new NativeParameterScope(parameters);
            fixed (byte* n = Name(name)) { var options = new NativeNodeParameters { Name = n, Local = scope.Pointer }; return new(this, CheckId(NativeMethods.ZL_Compressor_parameterizeNode(_handle, Id(node), &options))); }
        }
    }
    public OpenZlGraphReference ParameterizeGraph(OpenZlGraphReference graph, OpenZlLocalParameters? parameters = null, IReadOnlyList<OpenZlGraphReference>? customGraphs = null, IReadOnlyList<OpenZlNodeReference>? customNodes = null, string? name = null)
    {
        lock (_gate)
        {
            Alive(); using var scope = new NativeParameterScope(parameters);
            uint[] graphs = new uint[customGraphs?.Count ?? 0], nodes = new uint[customNodes?.Count ?? 0];
            for (int i = 0; i < graphs.Length; i++) graphs[i] = Id(customGraphs![i]);
            for (int i = 0; i < nodes.Length; i++) nodes[i] = Id(customNodes![i]);
            fixed (byte* n = Name(name)) fixed (uint* g = graphs, c = nodes)
            {
                var options = new NativeGraphParameters { Name = n, Local = scope.Pointer, Graphs = g, GraphCount = (nuint)graphs.Length, Nodes = c, NodeCount = (nuint)nodes.Length };
                return new(this, CheckId(NativeMethods.ZL_Compressor_parameterizeGraph(_handle, Id(graph), &options)));
            }
        }
    }
    public OpenZlGraphReference RegisterPipeline(IReadOnlyList<OpenZlNodeReference> nodes, OpenZlGraphReference successor)
    {
        ArgumentNullException.ThrowIfNull(nodes);
        lock (_gate)
        {
            Alive(); if (nodes.Count == 0) throw new ArgumentException("At least one node is required.", nameof(nodes));
            uint[] ids = new uint[nodes.Count]; for (int i = 0; i < ids.Length; i++) ids[i] = Id(nodes[i]);
            fixed (uint* p = ids) return Registered(NativeMethods.ZL_Compressor_registerStaticGraph_fromPipelineNodes1o(_handle, p, (nuint)ids.Length, Id(successor)));
        }
    }
    public OpenZlGraphReference GetGraph(OpenZlGraph graph)
    {
        lock (_gate) { Alive(); if (!Enum.IsDefined(graph) || (uint)graph < 2) throw new ArgumentOutOfRangeException(nameof(graph)); return new(this, (uint)graph); }
    }
    public OpenZlGraphReference RegisterPipeline(ReadOnlySpan<OpenZlNode> nodes, OpenZlGraphReference successor)
    {
        lock (_gate)
        {
            Alive(); if (nodes.IsEmpty) throw new ArgumentException("At least one node is required.", nameof(nodes));
            uint[] ids = new uint[nodes.Length];
            for (int i = 0; i < ids.Length; i++) { if (!Enum.IsDefined(nodes[i]) || (uint)nodes[i] < 2) throw new ArgumentOutOfRangeException(nameof(nodes)); ids[i] = (uint)nodes[i]; }
            fixed (uint* p = ids) return Registered(NativeMethods.ZL_Compressor_registerStaticGraph_fromPipelineNodes1o(_handle, p, (nuint)ids.Length, Id(successor)));
        }
    }
    public OpenZlGraphReference RegisterGraph(OpenZlNode node, IReadOnlyList<OpenZlGraphReference> successors)
    {
        ArgumentNullException.ThrowIfNull(successors);
        lock (_gate)
        {
            Alive(); if (!Enum.IsDefined(node) || (uint)node < 2) throw new ArgumentOutOfRangeException(nameof(node));
            uint[] ids = new uint[successors.Count]; for (int i = 0; i < ids.Length; i++) ids[i] = Id(successors[i]);
            fixed (uint* p = ids) return Registered(NativeMethods.ZL_Compressor_registerStaticGraph_fromNode(_handle, (uint)node, p, (nuint)ids.Length));
        }
    }
    private OpenZlGraphReference Registered(uint id) => id == 0 ? throw new OpenZlException(1, "Native graph registration failed.") : new(this, id);
    public void SelectGraph(OpenZlGraphReference graph)
    {
        lock (_gate) { Alive(); NativeErrors.Check(NativeMethods.ZL_Compressor_selectStartingGraphID(_handle, Id(graph)), r => NativeMethods.ZL_Compressor_getErrorContextString(_handle, r)); }
    }
    public byte[] Compress(ReadOnlySpan<byte> data, OpenZlCompressionOptions? options = null) => Compress(new[] { new OpenZlData(data) }, options);
    public byte[] Compress(IReadOnlyList<OpenZlData> inputs, OpenZlCompressionOptions? options = null)
    {
        ArgumentNullException.ThrowIfNull(inputs); options ??= new();
        if (inputs.Count == 0 || inputs.Count > 2048) throw new ArgumentOutOfRangeException(nameof(inputs));
        if (options.MaxCompressedBytes < 1) throw new ArgumentOutOfRangeException(nameof(options.MaxCompressedBytes));
        lock (_gate)
        {
            Alive(); var pinned = new NativeInput?[inputs.Count]; var refs = new nint[inputs.Count]; long total = 0;
            try
            {
                for (int i = 0; i < inputs.Count; i++) { ArgumentNullException.ThrowIfNull(inputs[i]); total = checked(total + inputs[i].Bytes.Length); pinned[i] = new(inputs[i]); refs[i] = pinned[i]!.Handle; }
                int capacity = (int)Math.Min(options.MaxCompressedBytes, Math.Max(256, total + total / 100 + 65536));
                while (true)
                {
                    using var ctx = new CompressionContextHandle();
                    nuint Check(NativeReport r) => NativeErrors.Check(r, e => NativeMethods.ZL_CCtx_getErrorContextString(ctx, e));
                    Check(NativeMethods.ZL_CCtx_refCompressor(ctx, _handle));
                    void Param(int id, int value) => Check(NativeMethods.ZL_CCtx_setParameter(ctx, id, value));
                    Param(2, options.CompressionLevel); Param(4, options.FormatVersion); Param(5, options.Permissive ? 1 : 2);
                    Param(6, options.CompressedChecksum ? 1 : 2); Param(7, options.ContentChecksum ? 1 : 2);
                    Param(11, options.MinimumStreamSize); Param(12, options.StoreOnExpansion ? 1 : 2);
                    if (options.Comment is { } comment) { byte[] utf8 = Encoding.UTF8.GetBytes(comment); fixed (byte* c = utf8) Check(NativeMethods.ZL_CCtx_addHeaderComment(ctx, c, (nuint)utf8.Length)); }
                    byte[] output = new byte[capacity]; NativeReport result;
                    fixed (byte* dst = output) fixed (nint* src = refs) result = NativeMethods.ZL_CCtx_compressMultiTypedRef(ctx, dst, (nuint)capacity, src, (nuint)refs.Length);
                    if (result.Code == 5 && capacity < options.MaxCompressedBytes) { capacity = (int)Math.Min(options.MaxCompressedBytes, (long)capacity * 2); continue; }
                    int written = checked((int)Check(result)); Array.Resize(ref output, written); return output;
                }
            }
            finally { foreach (var input in pinned) input?.Dispose(); }
        }
    }
    public void LoadDictionaryBundle(ReadOnlySpan<byte> bundle)
    {
        lock (_gate)
        {
            Alive(); fixed (byte* data = bundle) NativeErrors.Check(NativeMethods.ZL_Compressor_loadDictBundle(_handle, data, (nuint)bundle.Length), r => NativeMethods.ZL_Compressor_getErrorContextString(_handle, r));
        }
    }
    public byte[] Serialize() => Serialize(false);
    public string ToJson() => Encoding.UTF8.GetString(Serialize(true)).TrimEnd('\0');
    private byte[] Serialize(bool json)
    {
        lock (_gate)
        {
            Alive(); using var serializer = new SerializerHandle(); byte* data = null; nuint size = 0;
            var r = json ? NativeMethods.ZL_CompressorSerializer_serializeToJson(serializer, _handle, &data, &size) : NativeMethods.ZL_CompressorSerializer_serialize(serializer, _handle, &data, &size);
            NativeErrors.Check(r, e => NativeMethods.ZL_CompressorSerializer_getErrorContextString(serializer, e));
            return new ReadOnlySpan<byte>(data, checked((int)size)).ToArray();
        }
    }
    public void Dispose() { lock (_gate) _handle.Dispose(); }
}
