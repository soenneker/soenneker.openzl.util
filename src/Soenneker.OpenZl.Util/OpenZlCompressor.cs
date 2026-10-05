using System;
using System.Buffers;
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
    private CompressionContextHandle? _context;
    private OpenZlGraph? _standardGraph;
    internal void SelectStandardGraph(OpenZlGraph graph)
    {
        if (_standardGraph == graph) return;
        SelectGraph(GetGraph(graph));
        _standardGraph = graph;
    }
    internal OpenZlCompressor(OpenZlGraph graph) { try { SelectStandardGraph(graph); } catch { _handle.Dispose(); throw; } }
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
        lock (_gate) { Alive(); NativeErrors.Check(NativeMethods.ZL_Compressor_selectStartingGraphID(_handle, Id(graph)), r => NativeMethods.ZL_Compressor_getErrorContextString(_handle, r)); _standardGraph = null; }
    }
    public byte[] Compress(ReadOnlySpan<byte> data, OpenZlCompressionOptions? options = null)
    {
        options ??= OpenZlCompressionOptions.Default;
        Validate(options);
        int commentSize = options.Comment == null ? 0 : Encoding.UTF8.GetByteCount(options.Comment);
        int capacity = (int)Math.Min(options.MaxCompressedBytes, Math.Max(64, (long)data.Length + 64 + ((long)data.Length / 32768 + 1) * 16 + commentSize));
        lock (_gate)
        {
            Alive();
            while (true)
            {
                byte[] buffer = ArrayPool<byte>.Shared.Rent(capacity);
                try
                {
                    if (TryCompressCore(data, buffer.AsSpan(0, capacity), out int written, options)) return buffer.AsSpan(0, written).ToArray();
                    if (capacity == options.MaxCompressedBytes) throw new OpenZlException(5, "Compressed output exceeds the configured limit.");
                    capacity = (int)Math.Min(options.MaxCompressedBytes, (long)capacity * 2);
                }
                finally { ArrayPool<byte>.Shared.Return(buffer); }
            }
        }
    }
    public bool TryCompress(ReadOnlySpan<byte> data, Span<byte> destination, out int written, OpenZlCompressionOptions? options = null)
    {
        options ??= OpenZlCompressionOptions.Default;
        Validate(options);
        if (data.Overlaps(destination)) throw new ArgumentException("Input and output must not overlap.", nameof(destination));
        lock (_gate)
        {
            Alive();
            return TryCompressCore(data, destination.Slice(0, Math.Min(destination.Length, options.MaxCompressedBytes)), out written, options);
        }
    }
    private bool TryCompressCore(ReadOnlySpan<byte> data, Span<byte> destination, out int written, OpenZlCompressionOptions options)
    {
        written = 0;
        try
        {
            CompressionContextHandle ctx = Configure(options);
            NativeReport result;
            fixed (byte* src = data, dst = destination) result = NativeMethods.ZL_CCtx_compress(ctx, dst, (nuint)destination.Length, src, (nuint)data.Length);
            if (result.Code == 5) { ResetContext(); return false; }
            written = checked((int)NativeErrors.CheckCompression(result, ctx));
            return true;
        }
        catch { ResetContext(); throw; }
    }
    private CompressionContextHandle Configure(OpenZlCompressionOptions options)
    {
        CompressionContextHandle ctx = _context ??= new CompressionContextHandle();
        NativeErrors.CheckCompression(NativeMethods.ZL_CCtx_refCompressor(ctx, _handle), ctx);
        SetParameter(ctx, 2, options.CompressionLevel);
        SetParameter(ctx, 4, options.FormatVersion);
        SetParameter(ctx, 5, options.Permissive ? 1 : 2);
        SetParameter(ctx, 6, options.CompressedChecksum ? 1 : 2);
        SetParameter(ctx, 7, options.ContentChecksum ? 1 : 2);
        SetParameter(ctx, 11, options.MinimumStreamSize);
        SetParameter(ctx, 12, options.StoreOnExpansion ? 1 : 2);
        if (options.Comment is { } comment)
        {
            int length = Encoding.UTF8.GetByteCount(comment);
            byte[]? rented = length > 1024 ? ArrayPool<byte>.Shared.Rent(length) : null;
            Span<byte> utf8 = rented == null ? stackalloc byte[length] : rented.AsSpan(0, length);
            try
            {
                Encoding.UTF8.GetBytes(comment, utf8);
                fixed (byte* c = utf8) NativeErrors.CheckCompression(NativeMethods.ZL_CCtx_addHeaderComment(ctx, c, (nuint)length), ctx);
            }
            finally { if (rented != null) ArrayPool<byte>.Shared.Return(rented); }
        }
        return ctx;
    }
    private static void SetParameter(CompressionContextHandle ctx, int parameter, int value) => NativeErrors.CheckCompression(NativeMethods.ZL_CCtx_setParameter(ctx, parameter, value), ctx);
    private static void Validate(OpenZlCompressionOptions options)
    {
        if (options.MaxCompressedBytes < 1) throw new ArgumentOutOfRangeException(nameof(options.MaxCompressedBytes));
    }
    private void ResetContext() { _context?.Dispose(); _context = null; }
    public byte[] Compress(IReadOnlyList<OpenZlData> inputs, OpenZlCompressionOptions? options = null)
    {
        ArgumentNullException.ThrowIfNull(inputs);
        options ??= OpenZlCompressionOptions.Default;
        Validate(options);
        int count = inputs.Count;
        if (count == 0 || count > 2048) throw new ArgumentOutOfRangeException(nameof(inputs));
        lock (_gate)
        {
            Alive();
            var pinned = ArrayPool<NativeInput?>.Shared.Rent(count);
            Span<nint> refs = stackalloc nint[count];
            int initialized = 0;
            long total = 0;
            try
            {
                for (int i = 0; i < count; i++)
                {
                    OpenZlData input = inputs[i];
                    ArgumentNullException.ThrowIfNull(input);
                    total = checked(total + input.Bytes.Length);
                    pinned[i] = new NativeInput(input);
                    initialized++;
                    refs[i] = pinned[i]!.Handle;
                }
                int capacity = (int)Math.Min(options.MaxCompressedBytes, Math.Max(256, total + total / 100 + 65536));
                while (true)
                {
                    byte[] buffer = ArrayPool<byte>.Shared.Rent(capacity);
                    try
                    {
                        CompressionContextHandle ctx = Configure(options);
                        NativeReport result;
                        fixed (byte* dst = buffer) fixed (nint* src = refs) result = NativeMethods.ZL_CCtx_compressMultiTypedRef(ctx, dst, (nuint)capacity, src, (nuint)count);
                        if (result.Code == 5 && capacity < options.MaxCompressedBytes)
                        {
                            ResetContext();
                            capacity = (int)Math.Min(options.MaxCompressedBytes, (long)capacity * 2);
                            continue;
                        }
                        int written = checked((int)NativeErrors.CheckCompression(result, ctx));
                        return buffer.AsSpan(0, written).ToArray();
                    }
                    finally { ArrayPool<byte>.Shared.Return(buffer); }
                }
            }
            catch { ResetContext(); throw; }
            finally
            {
                for (int i = 0; i < initialized; i++) { pinned[i]!.Dispose(); pinned[i] = null; }
                ArrayPool<NativeInput?>.Shared.Return(pinned, clearArray: true);
            }
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
    public void Dispose() { lock (_gate) { ResetContext(); _handle.Dispose(); } }
}
