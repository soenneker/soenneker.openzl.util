using System;
using System.Collections.Generic;
using Soenneker.OpenZl.Util.Options;
namespace Soenneker.OpenZl.Util.Abstract;
/// <summary>Owns a native compression graph. Operations are serialized; dispose after use.</summary>
public interface IOpenZlCompressor : IDisposable
{
    /// <summary>Gets a built-in graph reference belonging to this compressor.</summary>
    OpenZlGraphReference GetGraph(OpenZlGraph graph);
    /// <summary>Gets a built-in node reference belonging to this compressor.</summary>
    OpenZlNodeReference GetNode(OpenZlNode node);
    /// <summary>Creates a serial-to-fixed-width conversion node.</summary>
    OpenZlNodeReference RegisterStructConversion(int width);
    /// <summary>Clones a node with copied native parameters and an optional diagnostic name.</summary>
    OpenZlNodeReference ParameterizeNode(OpenZlNodeReference node, OpenZlLocalParameters parameters, string? name = null);
    /// <summary>Clones a graph with native parameters, successor overrides, and an optional diagnostic name.</summary>
    OpenZlGraphReference ParameterizeGraph(OpenZlGraphReference graph, OpenZlLocalParameters? parameters = null, IReadOnlyList<OpenZlGraphReference>? customGraphs = null, IReadOnlyList<OpenZlNodeReference>? customNodes = null, string? name = null);
    /// <summary>Chains standard or parameterized single-output nodes into a successor graph.</summary>
    OpenZlGraphReference RegisterPipeline(IReadOnlyList<OpenZlNodeReference> nodes, OpenZlGraphReference successor);
    /// <summary>Chains single-output standard nodes into a successor graph.</summary>
    OpenZlGraphReference RegisterPipeline(ReadOnlySpan<OpenZlNode> nodes, OpenZlGraphReference successor);
    /// <summary>Connects each output of a standard node to a successor graph.</summary>
    OpenZlGraphReference RegisterGraph(OpenZlNode node, IReadOnlyList<OpenZlGraphReference> successors);
    /// <summary>Validates and selects the graph used by subsequent compression operations.</summary>
    void SelectGraph(OpenZlGraphReference graph);
    /// <summary>Compresses one serial byte stream. The Graph option is ignored; the selected compressor graph is used.</summary>
    byte[] Compress(ReadOnlySpan<byte> data, OpenZlCompressionOptions? options = null);
    /// <summary>Compresses a serial stream into caller-owned storage. Returns false with written = 0 when capacity or MaxCompressedBytes is insufficient. Input and output must not overlap. Destination contents are undefined on failure.</summary>
    bool TryCompress(ReadOnlySpan<byte> data, Span<byte> destination, out int written, OpenZlCompressionOptions? options = null);
    /// <summary>Compresses typed streams together in order. The selected graph must accept the input types and count.</summary>
    byte[] Compress(IReadOnlyList<OpenZlData> inputs, OpenZlCompressionOptions? options = null);
    /// <summary>Loads a serialized fat dictionary bundle for standard codecs.</summary>
    void LoadDictionaryBundle(ReadOnlySpan<byte> bundle);
    /// <summary>Serializes the graph configuration, which can be loaded with DeserializeCompressor.</summary>
    byte[] Serialize();
    /// <summary>Returns the native diagnostic JSON graph representation. JSON cannot be deserialized by OpenZL.</summary>
    string ToJson();
}
