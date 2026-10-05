namespace Soenneker.OpenZl.Util;
/// <summary>A node registered on a specific compressor.</summary>
public sealed class OpenZlNodeReference
{
    internal OpenZlCompressor Owner { get; }
    internal uint Id { get; }
    internal OpenZlNodeReference(OpenZlCompressor owner, uint id) { Owner = owner; Id = id; }
}
