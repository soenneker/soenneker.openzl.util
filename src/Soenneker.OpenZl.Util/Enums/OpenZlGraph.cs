using Soenneker.Gen.EnumValues;

namespace Soenneker.OpenZl.Util.Enums;

/// <summary>Standard graph identifiers for the bundled OpenZL revision.</summary>
[EnumValue<uint>]
public sealed partial class OpenZlGraph
{
    public static readonly OpenZlGraph Store = new(2);
    public static readonly OpenZlGraph Fse = new(3);
    public static readonly OpenZlGraph Huffman = new(4);
    public static readonly OpenZlGraph HuffmanHuf0 = new(5);
    public static readonly OpenZlGraph HuffmanPivco = new(6);
    public static readonly OpenZlGraph Entropy = new(7);
    public static readonly OpenZlGraph Constant = new(8);
    public static readonly OpenZlGraph Zstd = new(9);
    public static readonly OpenZlGraph Bitpack = new(10);
    public static readonly OpenZlGraph Flatpack = new(11);
    public static readonly OpenZlGraph FieldLz = new(12);
    public static readonly OpenZlGraph CompressGeneric = new(13);
    public static readonly OpenZlGraph SelectGenericLzBackend = new(14);
    public static readonly OpenZlGraph SegmentNumeric = new(15);
    public static readonly OpenZlGraph SelectNumeric = new(16);
    public static readonly OpenZlGraph MlSelector = new(17);
    public static readonly OpenZlGraph Clustering = new(18);
    public static readonly OpenZlGraph TryParseInt = new(19);
    public static readonly OpenZlGraph SimpleDataDescriptionLanguage = new(20);
    public static readonly OpenZlGraph SimpleDataDescriptionLanguageV2 = new(21);
    public static readonly OpenZlGraph Lz4 = new(22);
    public static readonly OpenZlGraph PartitionBitpack = new(23);
    public static readonly OpenZlGraph SegmentNum8FromSerial = new(24);
    public static readonly OpenZlGraph SegmentNum16FromSerial = new(25);
    public static readonly OpenZlGraph SegmentNum32FromSerial = new(26);
    public static readonly OpenZlGraph SegmentNum64FromSerial = new(27);
    public static readonly OpenZlGraph Lz = new(28);
    public static readonly OpenZlGraph SegmentSerial = new(29);
    public static readonly OpenZlGraph TransformerNumeric = new(30);
    public static readonly OpenZlGraph BruteForce = new(31);
}