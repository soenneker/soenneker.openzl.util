namespace Soenneker.OpenZl.Util;
/// <summary>A graph registered on a specific compressor.</summary>
public sealed class OpenZlGraphReference
{
    internal OpenZlCompressor Owner { get; }
    internal uint Id { get; }
    internal OpenZlGraphReference(OpenZlCompressor owner, uint id) { Owner = owner; Id = id; }
}
