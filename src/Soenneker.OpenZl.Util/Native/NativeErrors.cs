using System;
using System.Runtime.InteropServices;
namespace Soenneker.OpenZl.Util.Native;

internal static class NativeErrors
{
    internal static nuint Check(NativeReport report, Func<NativeReport, nint>? message = null)
    {
        if (report.Code != 0) throw new OpenZlException(report.Code, message == null ? $"OpenZL error {report.Code}." : Marshal.PtrToStringUTF8(message(report)) ?? $"OpenZL error {report.Code}.");
        return report.Value;
    }
    internal static nuint CheckCompression(NativeReport report, CompressionContextHandle context)
    {
        if (report.Code != 0) throw new OpenZlException(report.Code, Marshal.PtrToStringUTF8(NativeMethods.ZL_CCtx_getErrorContextString(context, report)) ?? $"OpenZL error {report.Code}.");
        return report.Value;
    }
    internal static nuint CheckDecompression(NativeReport report, DecompressionContextHandle context)
    {
        if (report.Code != 0) throw new OpenZlException(report.Code, Marshal.PtrToStringUTF8(NativeMethods.ZL_DCtx_getErrorContextString(context, report)) ?? $"OpenZL error {report.Code}.");
        return report.Value;
    }
}
