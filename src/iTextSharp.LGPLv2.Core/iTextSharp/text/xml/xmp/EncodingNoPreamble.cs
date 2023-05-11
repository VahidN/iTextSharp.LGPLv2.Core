using System.Text;

namespace iTextSharp.text.xml.xmp;

/// <summary>
///     A wrapper for an Encoding to suppress the preamble.
/// </summary>
public class EncodingNoPreamble : Encoding
{
    private static readonly byte[] _emptyPreamble = Array.Empty<byte>();
    private readonly Encoding _encoding;

    public EncodingNoPreamble(Encoding encoding) => _encoding = encoding;

    public override int CodePage => _encoding.CodePage;

    public override string EncodingName => _encoding.EncodingName;

    public override string WebName => _encoding.WebName;

    public override int GetByteCount(char[] chars, int index, int count) => _encoding.GetByteCount(chars, index, count);

    public override int GetBytes(char[] chars, int charIndex, int charCount, byte[] bytes, int byteIndex) =>
        _encoding.GetBytes(chars, charIndex, charCount, bytes, byteIndex);

    public override int GetCharCount(byte[] bytes, int index, int count) => _encoding.GetCharCount(bytes, index, count);

    public override int GetChars(byte[] bytes, int byteIndex, int byteCount, char[] chars, int charIndex) =>
        _encoding.GetChars(bytes, byteIndex, byteCount, chars, charIndex);

    public override Decoder GetDecoder() => _encoding.GetDecoder();

    public override Encoder GetEncoder() => _encoding.GetEncoder();

    public override int GetMaxByteCount(int charCount) => _encoding.GetMaxByteCount(charCount);

    public override int GetMaxCharCount(int byteCount) => _encoding.GetMaxCharCount(byteCount);

    public override byte[] GetPreamble() => _emptyPreamble;
}