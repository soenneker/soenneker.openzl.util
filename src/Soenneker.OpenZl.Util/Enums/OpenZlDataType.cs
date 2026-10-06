using Soenneker.Gen.EnumValues;

namespace Soenneker.OpenZl.Util.Enums;

/// <summary>OpenZL native stream kinds.</summary>
[EnumValue]
public sealed partial class OpenZlDataType
{
    public static readonly OpenZlDataType Serial = new(1);
    public static readonly OpenZlDataType Struct = new(2);
    public static readonly OpenZlDataType Numeric = new(4);
    public static readonly OpenZlDataType String = new(8);
}