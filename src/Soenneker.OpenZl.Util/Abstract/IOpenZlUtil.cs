using System;
using System.Collections.Generic;
using Soenneker.OpenZl.Util.Options;
using Soenneker.OpenZl.Util.Results;
namespace Soenneker.OpenZl.Util.Abstract;
/// <summary>Direct native OpenZL compression, decompression, frame inspection, and graph configuration.</summary>
public interface IOpenZlUtil
{
    /// <summary>Creates an owned compressor using a standard graph.</summary>
    IOpenZlCompressor CreateCompressor(OpenZlGraph graph = OpenZlGraph.CompressGeneric);
    /// <summary>Loads a serialized native graph configuration, optionally resolving a fat dictionary bundle.</summary>
    IOpenZlCompressor DeserializeCompressor(ReadOnlySpan<byte> data, ReadOnlySpan<byte> dictionaryBundle = default);
    /// <summary>Compresses a single serial byte stream.</summary>
    byte[] Compress(ReadOnlySpan<byte> data, OpenZlCompressionOptions? options = null);
    /// <summary>Compresses typed streams in order using the selected standard graph.</summary>
    byte[] Compress(IReadOnlyList<OpenZlData> inputs, OpenZlCompressionOptions? options = null);
    /// <summary>Decompresses a frame containing exactly one serial byte stream. Rejects trailing data.</summary>
    byte[] Decompress(ReadOnlySpan<byte> frame, OpenZlDecompressionOptions? options = null);
    /// <summary>Decompresses all typed outputs in order. Rejects trailing data and outputs exceeding the configured limits.</summary>
    IReadOnlyList<OpenZlData> DecompressTyped(ReadOnlySpan<byte> frame, OpenZlDecompressionOptions? options = null);
    /// <summary>Inspects one complete frame without decompressing it. Rejects trailing data and applies output limits.</summary>
    OpenZlFrameInfo Inspect(ReadOnlySpan<byte> frame, OpenZlDecompressionOptions? options = null);
}
