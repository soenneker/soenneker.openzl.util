using Soenneker.Gen.EnumValues;

namespace Soenneker.OpenZl.Util.Enums;
/// <summary>Standard node identifiers for the bundled OpenZL revision.</summary>
[EnumValue<uint>]
public sealed partial class OpenZlNode
{
    public static readonly OpenZlNode DeltaInt = new(2);
    public static readonly OpenZlNode TransposeSplit = new(3);
    public static readonly OpenZlNode Zigzag = new(4);
    public static readonly OpenZlNode DispatchNByTag = new(5);
    public static readonly OpenZlNode Float32Deconstruct = new(6);
    public static readonly OpenZlNode Bfloat16Deconstruct = new(7);
    public static readonly OpenZlNode Float16Deconstruct = new(8);
    public static readonly OpenZlNode FieldLz = new(9);
    public static readonly OpenZlNode ConvertStructToSerial = new(10);
    public static readonly OpenZlNode ConvertNumToStructLe = new(11);
    public static readonly OpenZlNode ConvertNumToSerialLe = new(12);
    public static readonly OpenZlNode ConvertSerialToStruct = new(13);
    public static readonly OpenZlNode ConvertSerialToStruct2 = new(14);
    public static readonly OpenZlNode ConvertSerialToStruct4 = new(15);
    public static readonly OpenZlNode ConvertSerialToStruct8 = new(16);
    public static readonly OpenZlNode ConvertStructToNumLe = new(17);
    public static readonly OpenZlNode ConvertStructToNumBe = new(18);
    public static readonly OpenZlNode ConvertSerialToNum8 = new(19);
    public static readonly OpenZlNode ConvertSerialToNumLe16 = new(20);
    public static readonly OpenZlNode ConvertSerialToNumLe32 = new(21);
    public static readonly OpenZlNode ConvertSerialToNumLe64 = new(22);
    public static readonly OpenZlNode ConvertSerialToNumBe16 = new(23);
    public static readonly OpenZlNode ConvertSerialToNumBe32 = new(24);
    public static readonly OpenZlNode ConvertSerialToNumBe64 = new(25);
    public static readonly OpenZlNode SeparateStringComponents = new(26);
    public static readonly OpenZlNode Bitunpack = new(27);
    public static readonly OpenZlNode RangePack = new(28);
    public static readonly OpenZlNode MergeSorted = new(29);
    public static readonly OpenZlNode Prefix = new(30);
    public static readonly OpenZlNode DivideBy = new(31);
    public static readonly OpenZlNode DispatchString = new(32);
    public static readonly OpenZlNode ConcatSerial = new(33);
    public static readonly OpenZlNode ConcatNum = new(34);
    public static readonly OpenZlNode ConcatStruct = new(35);
    public static readonly OpenZlNode ConcatString = new(36);
    public static readonly OpenZlNode DedupNum = new(37);
    public static readonly OpenZlNode ParseInt = new(38);
    public static readonly OpenZlNode InterleaveString = new(39);
    public static readonly OpenZlNode TokenizeStruct = new(40);
    public static readonly OpenZlNode TokenizeNumeric = new(41);
    public static readonly OpenZlNode TokenizeString = new(42);
    public static readonly OpenZlNode QuantizeOffsets = new(43);
    public static readonly OpenZlNode QuantizeLengths = new(44);
    public static readonly OpenZlNode BitsplitTop8 = new(45);
    public static readonly OpenZlNode BitsplitFp = new(46);
    public static readonly OpenZlNode BitsplitBf16 = new(47);
    public static readonly OpenZlNode Partition = new(48);
    public static readonly OpenZlNode SplitByrange = new(49);
    public static readonly OpenZlNode SentinelByte = new(50);
    public static readonly OpenZlNode SentinelNum = new(51);
    public static readonly OpenZlNode Lz = new(52);
    public static readonly OpenZlNode MuxLengths = new(53);
    public static readonly OpenZlNode SparseNum = new(54);
    public static readonly OpenZlNode SparseNumAuto = new(55);
}
