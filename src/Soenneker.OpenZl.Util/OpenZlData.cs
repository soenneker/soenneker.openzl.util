using System;
namespace Soenneker.OpenZl.Util;
/// <summary>An owned typed stream. Numeric bytes use native endianness; the bundled platforms are little-endian.</summary>
public sealed class OpenZlData
{
    /// <summary>Stream kind.</summary>
    public OpenZlDataType Type { get; }
    /// <summary>Owned stream bytes.</summary>
    public ReadOnlyMemory<byte> Bytes { get; }
    /// <summary>Element width for serial, struct, and numeric streams; zero for string streams.</summary>
    public int ElementWidth { get; }
    /// <summary>String byte lengths; empty for other stream kinds.</summary>
    public ReadOnlyMemory<uint> StringLengths { get; }
    /// <summary>Number of elements.</summary>
    public int Count => Type == OpenZlDataType.String ? StringLengths.Length : Bytes.Length / ElementWidth;
    /// <summary>Copies and validates stream data and dimensions.</summary>
    public OpenZlData(ReadOnlySpan<byte> bytes, OpenZlDataType type = OpenZlDataType.Serial, int elementWidth = 1, ReadOnlySpan<uint> stringLengths = default)
    {
        if (!Enum.IsDefined(type)) throw new ArgumentOutOfRangeException(nameof(type));
        if (type == OpenZlDataType.String)
        {
            long total = 0; foreach (uint n in stringLengths) total += n;
            if (total != bytes.Length) throw new ArgumentException("String lengths must exactly cover the data.", nameof(stringLengths));
            ElementWidth = 0; StringLengths = stringLengths.ToArray();
        }
        else
        {
            if (elementWidth <= 0 || bytes.Length % elementWidth != 0 || (type == OpenZlDataType.Serial && elementWidth != 1)
                || (type == OpenZlDataType.Numeric && elementWidth is not (1 or 2 or 4 or 8)) || !stringLengths.IsEmpty)
                throw new ArgumentException("Invalid typed stream dimensions.");
            ElementWidth = elementWidth;
        }
        Bytes = bytes.ToArray(); Type = type;
    }
}
