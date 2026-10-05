using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using Soenneker.OpenZl.Util.Abstract;
using Soenneker.OpenZl.Util.Native;
using Soenneker.OpenZl.Util.Options;
using Soenneker.OpenZl.Util.Results;
namespace Soenneker.OpenZl.Util;

public sealed unsafe class OpenZlUtil : IOpenZlUtil
{
    public OpenZlUtil()
    {
        if (RuntimeInformation.ProcessArchitecture != Architecture.X64 || (!OperatingSystem.IsWindows() && !OperatingSystem.IsLinux()))
            throw new PlatformNotSupportedException("Bundled OpenZL libraries support Windows x64 and glibc Linux x64.");
    }
    public IOpenZlCompressor CreateCompressor(OpenZlGraph graph = OpenZlGraph.CompressGeneric) => new OpenZlCompressor(graph);
    public IOpenZlCompressor DeserializeCompressor(ReadOnlySpan<byte> data, ReadOnlySpan<byte> dictionaryBundle = default) => new OpenZlCompressor(data, dictionaryBundle);
    public byte[] Compress(ReadOnlySpan<byte> data, OpenZlCompressionOptions? options = null)
    {
        using var compressor = CreateCompressor(options?.Graph ?? OpenZlGraph.CompressGeneric); return compressor.Compress(data, options);
    }
    public byte[] Compress(IReadOnlyList<OpenZlData> inputs, OpenZlCompressionOptions? options = null)
    {
        using var compressor = CreateCompressor(options?.Graph ?? OpenZlGraph.CompressGeneric); return compressor.Compress(inputs, options);
    }
    public byte[] Decompress(ReadOnlySpan<byte> frame, OpenZlDecompressionOptions? options = null)
    {
        var info = Inspect(frame, options);
        if (info.Outputs.Count != 1 || info.Outputs[0].Type != OpenZlDataType.Serial) throw new InvalidDataException("Frame requires typed decompression.");
        return DecompressTyped(frame, options)[0].Bytes.ToArray();
    }
    public OpenZlFrameInfo Inspect(ReadOnlySpan<byte> frame, OpenZlDecompressionOptions? options = null)
    {
        options ??= new(); Validate(options);
        if (frame.IsEmpty) throw new InvalidDataException("Frame is empty.");
        fixed (byte* src = frame)
        {
            long compressed = checked((long)NativeErrors.Check(NativeMethods.ZL_getCompressedSize(src, (nuint)frame.Length)));
            if (compressed != frame.Length) throw new InvalidDataException("Expected exactly one complete OpenZL frame.");
            long header = checked((long)NativeErrors.Check(NativeMethods.ZL_getHeaderSize(src, (nuint)frame.Length)));
            using var info = new FrameInfoHandle(frame);
            int version = checked((int)NativeErrors.Check(NativeMethods.ZL_FrameInfo_getFormatVersion(info)));
            nuint count = NativeErrors.Check(NativeMethods.ZL_FrameInfo_getNumOutputs(info));
            if (count > (nuint)options.MaxOutputs) throw new InvalidDataException("Frame exceeds the output stream limit.");
            var outputs = new OpenZlOutputInfo[(int)count]; long total = 0;
            for (int i = 0; i < outputs.Length; i++)
            {
                var type = (OpenZlDataType)NativeErrors.Check(NativeMethods.ZL_FrameInfo_getOutputType(info, i));
                long size = checked((long)NativeErrors.Check(NativeMethods.ZL_FrameInfo_getDecompressedSize(info, i)));
                long? elements = type == OpenZlDataType.Serial ? size : type == OpenZlDataType.String && version >= 21 ? checked((long)NativeErrors.Check(NativeMethods.ZL_FrameInfo_getNumElts(info, i))) : null;
                if (type == OpenZlDataType.String && elements == null) throw new InvalidDataException("String frames without element counts cannot be decoded with bounded allocation.");
                total = checked(total + size);
                if (total > options.MaxOutputBytes || (type == OpenZlDataType.String && elements > options.MaxStringCount)) throw new InvalidDataException("Frame exceeds the configured output limit.");
                outputs[i] = new(type, size, elements);
            }
            return new(version, compressed, header, Array.AsReadOnly(outputs));
        }
    }
    public IReadOnlyList<OpenZlData> DecompressTyped(ReadOnlySpan<byte> frame, OpenZlDecompressionOptions? options = null)
    {
        options ??= new(); var info = Inspect(frame, options);
        using var loader = options.DictionaryBundle.IsEmpty ? null : new FatBundleHandle();
        using var ctx = new DecompressionContextHandle();
        if (loader != null)
        {
            fixed (byte* bundle = options.DictionaryBundle.Span) NativeErrors.Check(NativeMethods.ZL_FatBundleDictLoader_loadFatBundle(loader, bundle, (nuint)options.DictionaryBundle.Length));
            NativeMethods.ZL_DCtx_refDictLoader(ctx, NativeMethods.ZL_FatBundleDictLoader_getDictLoader(loader));
        }
        void Check(NativeReport r) => NativeErrors.Check(r, e => NativeMethods.ZL_DCtx_getErrorContextString(ctx, e));
        Check(NativeMethods.ZL_DCtx_setParameter(ctx, 2, options.CheckCompressedChecksum ? 1 : 2));
        Check(NativeMethods.ZL_DCtx_setParameter(ctx, 3, options.CheckContentChecksum ? 1 : 2));
        Check(NativeMethods.ZL_DCtx_setParameter(ctx, 4, options.EnableCodecFusion ? 1 : 2));
        var buffers = new TypedBufferHandle?[info.Outputs.Count]; var refs = new nint[buffers.Length];
        try
        {
            for (int i = 0; i < buffers.Length; i++) { buffers[i] = new(); refs[i] = buffers[i]!.DangerousGetHandle(); }
            fixed (byte* src = frame) fixed (nint* dst = refs) Check(NativeMethods.ZL_DCtx_decompressMultiTBuffer(ctx, dst, (nuint)refs.Length, src, (nuint)frame.Length));
            var outputs = new OpenZlData[buffers.Length]; long total = 0;
            for (int i = 0; i < outputs.Length; i++)
            {
                var buffer = buffers[i]!; var type = NativeMethods.ZL_TypedBuffer_type(buffer);
                int size = checked((int)NativeMethods.ZL_TypedBuffer_byteSize(buffer));
                int count = checked((int)NativeMethods.ZL_TypedBuffer_numElts(buffer));
                total = checked(total + size);
                if (total > options.MaxOutputBytes || (type == OpenZlDataType.String && count > options.MaxStringCount)) throw new InvalidDataException("Decoded output exceeds configured limits.");
                var bytes = new ReadOnlySpan<byte>(NativeMethods.ZL_TypedBuffer_rPtr(buffer), size);
                var lengths = type == OpenZlDataType.String ? new ReadOnlySpan<uint>(NativeMethods.ZL_TypedBuffer_rStringLens(buffer), count) : default;
                int width = type == OpenZlDataType.String ? 0 : checked((int)NativeMethods.ZL_TypedBuffer_eltWidth(buffer));
                outputs[i] = new(bytes, type, width, lengths);
            }
            return Array.AsReadOnly(outputs);
        }
        finally { foreach (var buffer in buffers) buffer?.Dispose(); }
    }
    private static void Validate(OpenZlDecompressionOptions options)
    {
        if (options.MaxOutputBytes < 0 || options.MaxOutputs < 1 || options.MaxStringCount < 0) throw new ArgumentOutOfRangeException(nameof(options));
    }
}
