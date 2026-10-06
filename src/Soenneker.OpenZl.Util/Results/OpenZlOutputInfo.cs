using Soenneker.OpenZl.Util.Enums;
namespace Soenneker.OpenZl.Util.Results;
/// <summary>Describes an output stream declared by an OpenZL frame. ElementCount is null when the frame does not expose it.</summary>
public sealed record OpenZlOutputInfo(OpenZlDataType Type, long ByteSize, long? ElementCount);
