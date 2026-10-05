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
}
