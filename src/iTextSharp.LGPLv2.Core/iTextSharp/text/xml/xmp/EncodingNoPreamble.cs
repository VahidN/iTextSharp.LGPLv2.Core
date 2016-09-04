using System.Text;

namespace iTextSharp.text.xml.xmp
{

    /// <summary>
    /// A wrapper for an Encoding to suppress the preamble.
    /// </summary>
    public class EncodingNoPreamble : Encoding
    {

        private static readonly byte[] _emptyPreamble = new byte[0];
        private readonly Encoding _encoding;

        public EncodingNoPreamble(Encoding encoding)
        {
            _encoding = encoding;
        }

        public override int CodePage
        {
            get
            {
                return _encoding.CodePage;
            }
        }

        public override string EncodingName
        {
            get
            {
                return _encoding.EncodingName;
            }
        }

        public override string WebName
        {
            get
            {
                return _encoding.WebName;
            }
        }

        public override int GetByteCount(char[] chars, int index, int count)
        {
            return _encoding.GetByteCount(chars, index, count);
        }

        public override int GetBytes(char[] chars, int charIndex, int charCount, byte[] bytes, int byteIndex)
        {
            return _encoding.GetBytes(chars, charIndex, charCount, bytes, byteIndex);
        }

        public override int GetCharCount(byte[] bytes, int index, int count)
        {
            return _encoding.GetCharCount(bytes, index, count);
        }

        public override int GetChars(byte[] bytes, int byteIndex, int byteCount, char[] chars, int charIndex)
        {
            return _encoding.GetChars(bytes, byteIndex, byteCount, chars, charIndex);
        }

        public override Decoder GetDecoder()
        {
            return _encoding.GetDecoder();
        }

        public override Encoder GetEncoder()
        {
            return _encoding.GetEncoder();
        }

        public override int GetMaxByteCount(int charCount)
        {
            return _encoding.GetMaxByteCount(charCount);
        }

        public override int GetMaxCharCount(int byteCount)
        {
            return _encoding.GetMaxCharCount(byteCount);
        }
        public override byte[] GetPreamble()
        {
            return _emptyPreamble;
        }
    }
}