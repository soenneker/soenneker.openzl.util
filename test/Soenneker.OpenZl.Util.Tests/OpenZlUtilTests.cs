using Soenneker.OpenZl.Util.Enums;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Soenneker.OpenZl.Util.Options;
namespace Soenneker.OpenZl.Util.Tests;

public sealed class OpenZlUtilTests
{
    private readonly OpenZlUtil _util = new();
    private static void Require(bool condition) { if (!condition) throw new Exception("Assertion failed."); }
    [Test]
    public void RoundTripsAndMetadata()
    {
        foreach (var graph in new[] { OpenZlGraph.CompressGeneric, OpenZlGraph.Store, OpenZlGraph.Zstd, OpenZlGraph.Lz4 })
            foreach (int size in new[] { 0, 1, 65536 })
            {
                byte[] data = Enumerable.Range(0, size).Select(i => (byte)(i % 251)).ToArray();
                byte[] frame = _util.Compress(data, new() { Graph = graph, Comment = "P/Invoke test" });
                Require(data.SequenceEqual(_util.Decompress(frame)));
                var info = _util.Inspect(frame); Require(info.FormatVersion == 27 && info.Outputs.Count == 1 && info.Outputs[0].ByteSize == size);
            }
    }
    [Test]
    public void TypedStreamsAndGraphs()
    {
        var numeric = new OpenZlData(Enumerable.Range(0, 1000).SelectMany(BitConverter.GetBytes).ToArray(), OpenZlDataType.Numeric, 4);
        var strings = new OpenZlData(Encoding.UTF8.GetBytes("alphaalphabetabeta"), OpenZlDataType.String, stringLengths: new uint[] { 5, 5, 4, 4 });
        foreach (var input in new[] { numeric, strings, new OpenZlData(new byte[60], OpenZlDataType.Struct, 3) })
        {
            byte[] frame = _util.Compress(new[] { input }); var output = _util.DecompressTyped(frame)[0];
            Require(input.Type == output.Type && input.Bytes.Span.SequenceEqual(output.Bytes.Span) && input.StringLengths.Span.SequenceEqual(output.StringLengths.Span));
        }
        using var compressor = _util.CreateCompressor();
        var graph = compressor.RegisterPipeline(new[] { OpenZlNode.DeltaInt }, compressor.GetGraph(OpenZlGraph.Bitpack));
        compressor.SelectGraph(graph);
        byte[] compressed = compressor.Compress(new[] { numeric });
        Require(numeric.Bytes.Span.SequenceEqual(_util.DecompressTyped(compressed)[0].Bytes.Span));
        using var restored = _util.DeserializeCompressor(compressor.Serialize());
        Require(numeric.Bytes.Span.SequenceEqual(_util.DecompressTyped(restored.Compress(new[] { numeric }))[0].Bytes.Span));
        Require(compressor.ToJson().Length > 10);
    }
    [Test]
    public void MultipleOutputsAndParameterizedGraphs()
    {
        var inputs = new[] { new OpenZlData(new byte[123]), new OpenZlData(new byte[456]), new OpenZlData(new byte[12], OpenZlDataType.Numeric, 4) };
        var outputs = _util.DecompressTyped(_util.Compress(inputs));
        Require(outputs.Count == inputs.Length);
        for (int i = 0; i < inputs.Length; i++) Require(inputs[i].Type == outputs[i].Type && inputs[i].Bytes.Span.SequenceEqual(outputs[i].Bytes.Span));
        using var compressor = _util.CreateCompressor();
        var converted = compressor.ParameterizeNode(compressor.GetNode(OpenZlNode.ConvertSerialToStruct), new() { Integers = new System.Collections.Generic.Dictionary<int, int> { [1] = 3 } }, "triples");
        var successor = compressor.ParameterizeGraph(compressor.GetGraph(OpenZlGraph.CompressGeneric), name: "generic-triplets");
        compressor.SelectGraph(compressor.RegisterPipeline(new[] { converted }, successor));
        byte[] bytes = new byte[300]; Require(bytes.SequenceEqual(_util.Decompress(compressor.Compress(bytes))));
        compressor.SelectGraph(compressor.RegisterPipeline(new[] { compressor.RegisterStructConversion(3) }, successor));
        Require(bytes.SequenceEqual(_util.Decompress(compressor.Compress(bytes))));
    }
    [Test]
    public void OlderWireFormats()
    {
        byte[] data = new byte[8192];
        foreach (int version in new[] { 8, 14, 20, 21, 27 })
        {
            byte[] frame = _util.Compress(data, new() { FormatVersion = version, Graph = OpenZlGraph.Zstd });
            Require(data.SequenceEqual(_util.Decompress(frame)) && _util.Inspect(frame).FormatVersion == version);
        }
    }
    [Test]
    public void RejectsInvalidAndOversizedFrames()
    {
        byte[] frame = _util.Compress(new byte[10000]);
        bool failed = false; try { _util.Decompress(frame, new() { MaxOutputBytes = 100 }); } catch (InvalidDataException) { failed = true; }
        Require(failed);
        failed = false; try { _util.Decompress(frame.Concat(new byte[] { 1 }).ToArray()); } catch (InvalidDataException) { failed = true; }
        Require(failed);
        frame[frame.Length / 2] ^= 0x80; failed = false; try { _util.Decompress(frame); } catch (Exception e) when (e is OpenZlException or InvalidDataException) { failed = true; }
        Require(failed);
    }
    [Test]
    public void RejectsMalformedDictionaryBundles()
    {
        byte[] frame = _util.Compress(new byte[100]); bool failed = false;
        try { _util.Decompress(frame, new() { DictionaryBundle = new byte[] { 1, 2, 3 } }); } catch (OpenZlException) { failed = true; }
        Require(failed);
        using var compressor = _util.CreateCompressor(); failed = false;
        try { compressor.LoadDictionaryBundle(new byte[] { 1, 2, 3 }); } catch (OpenZlException) { failed = true; }
        Require(failed);
    }
    [Test]
    public void DisposalOwnershipAndConcurrency()
    {
        using var first = _util.CreateCompressor(); using var second = _util.CreateCompressor();
        bool failed = false; try { second.SelectGraph(first.GetGraph(OpenZlGraph.Store)); } catch (ArgumentException) { failed = true; }
        Require(failed);
        Parallel.For(0, 20, i => { byte[] data = new byte[1000 + i]; Require(data.SequenceEqual(_util.Decompress(first.Compress(data)))); });
        first.Dispose(); failed = false; try { first.Compress(new byte[1]); } catch (ObjectDisposedException) { failed = true; }
        Require(failed);
    }
}
