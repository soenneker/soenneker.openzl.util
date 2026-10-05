using System;
using System.Linq;
using System.Threading.Tasks;
using Soenneker.OpenZl.Util.Options;
namespace Soenneker.OpenZl.Util.Tests;

public sealed class OpenZlBufferTests
{
    private static void Require(bool condition) { if (!condition) throw new Exception("Assertion failed."); }

    [Test]
    public void ReusesBuffersAndRecoversAfterCapacityFailures()
    {
        using var util = new OpenZlUtil();
        foreach (int size in new[] { 0, 1, 31, 4096, 131072 })
        {
            byte[] input = new byte[size]; new Random(size).NextBytes(input);
            byte[] compressed = new byte[size + 65536], decoded = new byte[size + 16];
            Require(!util.TryCompress(input, Span<byte>.Empty, out int written) && written == 0);
            Require(util.TryCompress(input, compressed, out written));
            byte[] frame = compressed.AsSpan(0, written).ToArray();
            if (size > 0) Require(!util.TryDecompress(frame, new byte[size - 1], out int count) && count == 0);
            decoded.AsSpan().Fill(123);
            Require(util.TryDecompress(frame, decoded, out int restored) && restored == size);
            Require(input.AsSpan().SequenceEqual(decoded.AsSpan(0, restored)));
            Require(decoded.AsSpan(restored).IndexOfAnyExcept((byte)123) < 0);
            Require(util.Decompress(frame).AsSpan().SequenceEqual(input));
        }
    }

    [Test]
    public void OptionsAndCommentsDoNotLeakAcrossPooledCalls()
    {
        using var util = new OpenZlUtil();
        byte[] input = Enumerable.Range(0, 10000).Select(i => (byte)(i % 23)).ToArray();
        byte[] expected = util.Compress(input);
        foreach (var graph in new[] { OpenZlGraph.Store, OpenZlGraph.Zstd, OpenZlGraph.Lz4 })
        {
            var options = new OpenZlCompressionOptions { Graph = graph, CompressionLevel = 1, ContentChecksum = false, CompressedChecksum = false, Comment = new string('x', 2048) };
            Require(input.AsSpan().SequenceEqual(util.Decompress(util.Compress(input, options))));
            Require(expected.AsSpan().SequenceEqual(util.Compress(input)));
        }
        bool failed = false;
        try { util.Decompress(expected, new() { DictionaryBundle = new byte[] { 1, 2, 3 } }); } catch (OpenZlException) { failed = true; }
        Require(failed);
        Require(input.AsSpan().SequenceEqual(util.Decompress(expected)));
        byte[] corrupted = (byte[])expected.Clone(); corrupted[^1] ^= 0x80;
        failed = false;
        try { util.Decompress(corrupted); } catch (Exception) { failed = true; }
        Require(failed);
        Require(input.AsSpan().SequenceEqual(util.Decompress(expected)));
        Require(!util.TryCompress(input, new byte[20000], out int written, new() { MaxCompressedBytes = 1 }) && written == 0);
        Require(expected.AsSpan().SequenceEqual(util.Compress(input)));
    }

    [Test]
    public void OverlapAndDisposalAreRejected()
    {
        using var util = new OpenZlUtil();
        byte[] buffer = new byte[10000]; bool failed = false;
        try { util.TryCompress(buffer.AsSpan(0, 100), buffer, out _); } catch (ArgumentException) { failed = true; }
        Require(failed);
        byte[] frame = util.Compress(new byte[100]); frame.CopyTo(buffer, 0); failed = false;
        try { util.TryDecompress(buffer.AsSpan(0, frame.Length), buffer, out _); } catch (ArgumentException) { failed = true; }
        Require(failed);
        using var compressor = util.CreateCompressor();
        util.Dispose(); failed = false;
        try { util.TryCompress(new byte[1], buffer, out _); } catch (ObjectDisposedException) { failed = true; }
        Require(failed);
        failed = false;
        try { util.TryDecompress(frame, buffer, out _); } catch (ObjectDisposedException) { failed = true; }
        Require(failed);
        Require(compressor.Compress(new byte[1]).Length > 0);
    }

    [Test]
    public void ConcurrentCallsKeepGraphsAndOutputsIndependent()
    {
        using var util = new OpenZlUtil();
        Parallel.For(0, 100, i =>
        {
            byte[] input = new byte[1000 + i]; new Random(i).NextBytes(input);
            var options = new OpenZlCompressionOptions { Graph = i % 2 == 0 ? OpenZlGraph.Store : OpenZlGraph.Zstd, CompressionLevel = i % 6 + 1 };
            byte[] frame = util.Compress(input, options);
            Require(input.AsSpan().SequenceEqual(util.Decompress(frame)));
        });
    }
}
