using System.Runtime.InteropServices;
namespace Soenneker.OpenZl.Util.Native;

internal static unsafe class NativeMethods
{
    [DllImport("openzl", CallingConvention = CallingConvention.Cdecl)]
    internal static extern nint ZL_Compressor_create();
    [DllImport("openzl", CallingConvention = CallingConvention.Cdecl)]
    internal static extern void ZL_Compressor_free(nint value);
    [DllImport("openzl", CallingConvention = CallingConvention.Cdecl)]
    internal static extern nint ZL_Compressor_getErrorContextString(CompressorHandle context, NativeReport report);
    [DllImport("openzl", CallingConvention = CallingConvention.Cdecl)]
    internal static extern nint ZL_CCtx_create();
    [DllImport("openzl", CallingConvention = CallingConvention.Cdecl)]
    internal static extern void ZL_CCtx_free(nint value);
    [DllImport("openzl", CallingConvention = CallingConvention.Cdecl)]
    internal static extern nint ZL_CCtx_getErrorContextString(CompressionContextHandle context, NativeReport report);
    [DllImport("openzl", CallingConvention = CallingConvention.Cdecl)]
    internal static extern nint ZL_DCtx_create();
    [DllImport("openzl", CallingConvention = CallingConvention.Cdecl)]
    internal static extern void ZL_DCtx_free(nint value);
    [DllImport("openzl", CallingConvention = CallingConvention.Cdecl)]
    internal static extern nint ZL_DCtx_getErrorContextString(DecompressionContextHandle context, NativeReport report);
    [DllImport("openzl", CallingConvention = CallingConvention.Cdecl)]
    internal static extern nint ZL_CompressorSerializer_create();
    [DllImport("openzl", CallingConvention = CallingConvention.Cdecl)]
    internal static extern void ZL_CompressorSerializer_free(nint value);
    [DllImport("openzl", CallingConvention = CallingConvention.Cdecl)]
    internal static extern nint ZL_CompressorSerializer_getErrorContextString(SerializerHandle context, NativeReport report);
    [DllImport("openzl", CallingConvention = CallingConvention.Cdecl)]
    internal static extern nint ZL_CompressorDeserializer_create();
    [DllImport("openzl", CallingConvention = CallingConvention.Cdecl)]
    internal static extern void ZL_CompressorDeserializer_free(nint value);
    [DllImport("openzl", CallingConvention = CallingConvention.Cdecl)]
    internal static extern nint ZL_CompressorDeserializer_getErrorContextString(DeserializerHandle context, NativeReport report);
    [DllImport("openzl", CallingConvention = CallingConvention.Cdecl)]
    internal static extern nint ZL_TypedBuffer_create();
    [DllImport("openzl", CallingConvention = CallingConvention.Cdecl)]
    internal static extern void ZL_TypedBuffer_free(nint value);
    [DllImport("openzl", CallingConvention = CallingConvention.Cdecl)]
    internal static extern NativeReport ZL_Compressor_setParameter(CompressorHandle context, int parameter, int value);
    [DllImport("openzl", CallingConvention = CallingConvention.Cdecl)]
    internal static extern NativeReport ZL_CCtx_setParameter(CompressionContextHandle context, int parameter, int value);
    [DllImport("openzl", CallingConvention = CallingConvention.Cdecl)]
    internal static extern NativeReport ZL_DCtx_setParameter(DecompressionContextHandle context, int parameter, int value);
    [DllImport("openzl", CallingConvention = CallingConvention.Cdecl)]
    internal static extern NativeReport ZL_Compressor_selectStartingGraphID(CompressorHandle context, uint graph);
    [DllImport("openzl", CallingConvention = CallingConvention.Cdecl)]
    internal static extern uint ZL_Compressor_registerStaticGraph_fromPipelineNodes1o(CompressorHandle context, uint* nodes, nuint count, uint successor);
    [DllImport("openzl", CallingConvention = CallingConvention.Cdecl)]
    internal static extern uint ZL_Compressor_registerStaticGraph_fromNode(CompressorHandle context, uint node, uint* successors, nuint count);
    [DllImport("openzl", CallingConvention = CallingConvention.Cdecl)]
    internal static extern NativeReport ZL_CCtx_refCompressor(CompressionContextHandle context, CompressorHandle compressor);
    [DllImport("openzl", CallingConvention = CallingConvention.Cdecl)]
    internal static extern NativeReport ZL_CCtx_addHeaderComment(CompressionContextHandle context, byte* data, nuint size);
    [DllImport("openzl", CallingConvention = CallingConvention.Cdecl)]
    internal static extern NativeReport ZL_CCtx_compressMultiTypedRef(CompressionContextHandle context, byte* dst, nuint capacity, nint* inputs, nuint count);
    [DllImport("openzl", CallingConvention = CallingConvention.Cdecl)]
    internal static extern nint ZL_TypedRef_createSerial(void* data, nuint size);
    [DllImport("openzl", CallingConvention = CallingConvention.Cdecl)]
    internal static extern nint ZL_TypedRef_createStruct(void* data, nuint width, nuint count);
    [DllImport("openzl", CallingConvention = CallingConvention.Cdecl)]
    internal static extern nint ZL_TypedRef_createNumeric(void* data, nuint width, nuint count);
    [DllImport("openzl", CallingConvention = CallingConvention.Cdecl)]
    internal static extern nint ZL_TypedRef_createString(void* data, nuint size, uint* lengths, nuint count);
    [DllImport("openzl", CallingConvention = CallingConvention.Cdecl)]
    internal static extern void ZL_TypedRef_free(nint value);
    [DllImport("openzl", CallingConvention = CallingConvention.Cdecl)]
    internal static extern NativeReport ZL_DCtx_decompressMultiTBuffer(DecompressionContextHandle context, nint* outputs, nuint count, byte* src, nuint size);
    [DllImport("openzl", CallingConvention = CallingConvention.Cdecl)]
    internal static extern OpenZlDataType ZL_TypedBuffer_type(TypedBufferHandle buffer);
    [DllImport("openzl", CallingConvention = CallingConvention.Cdecl)]
    internal static extern nuint ZL_TypedBuffer_byteSize(TypedBufferHandle buffer);
    [DllImport("openzl", CallingConvention = CallingConvention.Cdecl)]
    internal static extern byte* ZL_TypedBuffer_rPtr(TypedBufferHandle buffer);
    [DllImport("openzl", CallingConvention = CallingConvention.Cdecl)]
    internal static extern nuint ZL_TypedBuffer_numElts(TypedBufferHandle buffer);
    [DllImport("openzl", CallingConvention = CallingConvention.Cdecl)]
    internal static extern nuint ZL_TypedBuffer_eltWidth(TypedBufferHandle buffer);
    [DllImport("openzl", CallingConvention = CallingConvention.Cdecl)]
    internal static extern uint* ZL_TypedBuffer_rStringLens(TypedBufferHandle buffer);
    [DllImport("openzl", CallingConvention = CallingConvention.Cdecl)]
    internal static extern nint ZL_FrameInfo_create(byte* src, nuint size);
    [DllImport("openzl", CallingConvention = CallingConvention.Cdecl)]
    internal static extern void ZL_FrameInfo_free(nint value);
    [DllImport("openzl", CallingConvention = CallingConvention.Cdecl)]
    internal static extern NativeReport ZL_FrameInfo_getFormatVersion(FrameInfoHandle info);
    [DllImport("openzl", CallingConvention = CallingConvention.Cdecl)]
    internal static extern NativeReport ZL_FrameInfo_getNumOutputs(FrameInfoHandle info);
    [DllImport("openzl", CallingConvention = CallingConvention.Cdecl)]
    internal static extern NativeReport ZL_FrameInfo_getOutputType(FrameInfoHandle info, int output);
    [DllImport("openzl", CallingConvention = CallingConvention.Cdecl)]
    internal static extern NativeReport ZL_FrameInfo_getDecompressedSize(FrameInfoHandle info, int output);
    [DllImport("openzl", CallingConvention = CallingConvention.Cdecl)]
    internal static extern NativeReport ZL_FrameInfo_getNumElts(FrameInfoHandle info, int output);
    [DllImport("openzl", CallingConvention = CallingConvention.Cdecl)]
    internal static extern NativeReport ZL_getCompressedSize(byte* src, nuint size);
    [DllImport("openzl", CallingConvention = CallingConvention.Cdecl)]
    internal static extern NativeReport ZL_getHeaderSize(byte* src, nuint size);
    [DllImport("openzl", CallingConvention = CallingConvention.Cdecl)]
    internal static extern NativeReport ZL_CompressorSerializer_serialize(SerializerHandle serializer, CompressorHandle compressor, byte** data, nuint* size);
    [DllImport("openzl", CallingConvention = CallingConvention.Cdecl)]
    internal static extern NativeReport ZL_CompressorSerializer_serializeToJson(SerializerHandle serializer, CompressorHandle compressor, byte** data, nuint* size);
    [DllImport("openzl", CallingConvention = CallingConvention.Cdecl)]
    internal static extern NativeReport ZL_CompressorDeserializer_deserialize(DeserializerHandle deserializer, CompressorHandle compressor, byte* data, nuint size, byte* bundle, nuint bundleSize);
    [DllImport("openzl", CallingConvention = CallingConvention.Cdecl)]
    internal static extern NativeIdResult ZL_Compressor_parameterizeNode(CompressorHandle context, uint node, NativeNodeParameters* parameters);
    [DllImport("openzl", CallingConvention = CallingConvention.Cdecl)]
    internal static extern NativeIdResult ZL_Compressor_parameterizeGraph(CompressorHandle context, uint graph, NativeGraphParameters* parameters);
    [DllImport("openzl", CallingConvention = CallingConvention.Cdecl)]
    internal static extern NativeIdResult ZL_Compressor_parameterizeConvertSerialToStructNode(CompressorHandle context, int width);
    [DllImport("openzl", CallingConvention = CallingConvention.Cdecl)]
    internal static extern nint ZL_FatBundleDictLoader_create();
    [DllImport("openzl", CallingConvention = CallingConvention.Cdecl)]
    internal static extern void ZL_FatBundleDictLoader_free(nint loader);
    [DllImport("openzl", CallingConvention = CallingConvention.Cdecl)]
    internal static extern NativeReport ZL_FatBundleDictLoader_loadFatBundle(FatBundleHandle loader, byte* data, nuint size);
    [DllImport("openzl", CallingConvention = CallingConvention.Cdecl)]
    internal static extern nint ZL_FatBundleDictLoader_getDictLoader(FatBundleHandle loader);
    [DllImport("openzl", CallingConvention = CallingConvention.Cdecl)]
    internal static extern void ZL_DCtx_refDictLoader(DecompressionContextHandle context, nint loader);
    [DllImport("openzl", CallingConvention = CallingConvention.Cdecl)]
    internal static extern NativeReport ZL_Compressor_loadDictBundle(CompressorHandle context, byte* data, nuint size);
}
