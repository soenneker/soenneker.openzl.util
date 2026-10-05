using System;
namespace Soenneker.OpenZl.Util.Options;
/// <summary>Controls output allocation and native verification.</summary>
public sealed class OpenZlDecompressionOptions
{
    /// <summary>Optional serialized fat dictionary bundle for standard native codecs. Custom codec callbacks are not supported.</summary>
    public ReadOnlyMemory<byte> DictionaryBundle { get; init; }
    /// <summary>Maximum total decoded bytes. This limits outputs, not the native decoder's intermediate allocations.</summary>
    public int MaxOutputBytes { get; init; } = 256 * 1024 * 1024;
    /// <summary>Maximum output streams accepted from one frame.</summary>
    public int MaxOutputs { get; init; } = 2048;
    /// <summary>Maximum string elements per output stream.</summary>
    public int MaxStringCount { get; init; } = 16 * 1024 * 1024;
    /// <summary>Verify compressed checksums when present.</summary>
    public bool CheckCompressedChecksum { get; init; } = true;
    /// <summary>Verify content checksums when present.</summary>
    public bool CheckContentChecksum { get; init; } = true;
    /// <summary>Enable native codec fusion.</summary>
    public bool EnableCodecFusion { get; init; } = true;
}
