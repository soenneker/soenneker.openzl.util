using System;
namespace Soenneker.OpenZl.Util;
/// <summary>An error returned by native OpenZL.</summary>
public sealed class OpenZlException(int errorCode, string message) : Exception(message)
{
    /// <summary>Upstream error code for the bundled native revision.</summary>
    public int ErrorCode { get; } = errorCode;
}
