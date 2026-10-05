using System.Collections.Generic;
namespace Soenneker.OpenZl.Util.Results;
/// <summary>Native-validated frame metadata.</summary>
public sealed record OpenZlFrameInfo(int FormatVersion, long CompressedSize, long HeaderSize, IReadOnlyList<OpenZlOutputInfo> Outputs);
