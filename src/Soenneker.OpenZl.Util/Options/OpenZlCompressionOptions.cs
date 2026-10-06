using Soenneker.OpenZl.Util.Enums;
namespace Soenneker.OpenZl.Util.Options;
/// <summary>Native compression parameters. Instances are read at the start of an operation.</summary>
public sealed class OpenZlCompressionOptions
{
    internal static readonly OpenZlCompressionOptions Default = new();

    /// <summary>Standard graph used by the convenience API.</summary>
    public OpenZlGraph Graph { get; init; } = OpenZlGraph.CompressGeneric;
    /// <summary>Wire format version; the bundled revision supports versions 8 through 27.</summary>
    public int FormatVersion { get; init; } = 27;
    /// <summary>Compression effort. The native graph interprets this value.</summary>
    public int CompressionLevel { get; init; } = 6;
    /// <summary>Include the compressed-data checksum.</summary>
    public bool CompressedChecksum { get; init; } = true;
    /// <summary>Include the decoded-data checksum.</summary>
    public bool ContentChecksum { get; init; } = true;
    /// <summary>Permit fallback when a graph rejects an input.</summary>
    public bool Permissive { get; init; }
    /// <summary>Store data when compression would expand it.</summary>
    public bool StoreOnExpansion { get; init; } = true;
    /// <summary>Native minimum stream size. Negative values disable early storage.</summary>
    public int MinimumStreamSize { get; init; }
    /// <summary>Optional UTF-8 comment. Requires format version 22 or later.</summary>
    public string? Comment { get; init; }
    /// <summary>Maximum allocation for compressed output, including retry growth.</summary>
    public int MaxCompressedBytes { get; init; } = 512 * 1024 * 1024;
}
