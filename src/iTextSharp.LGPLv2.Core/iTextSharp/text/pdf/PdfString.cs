namespace iTextSharp.text.pdf;

/// <summary>
///     A  PdfString -class is the PDF-equivalent of a JAVA- string -object.
///     A string is a sequence of characters delimited by parenthesis. If a string is too long
///     to be conveniently placed on a single line, it may be split across multiple lines by using
///     the backslash character (\) at the end of a line to indicate that the string continues
///     on the following line. Within a string, the backslash character is used as an escape to
///     specify unbalanced parenthesis, non-printing ASCII characters, and the backslash character
///     itself. Use of the \<I>ddd</I> escape sequence is the preferred way to represent characters
///     outside the printable ASCII character set.
///     This object is described in the 'Portable Document Format Reference Manual version 1.3'
///     section 4.4 (page 37-39).
///     @see        PdfObject
///     @see        BadPdfFormatException
/// </summary>
public class PdfString : PdfObject
{
    /// <summary>
    ///     membervariables
    /// </summary>
    /// <summary>
    ///     The encoding.
    /// </summary>
    protected string encoding = TEXT_PDFDOCENCODING;

    protected bool HexWriting;

    protected int ObjGen;

    protected int ObjNum;

    protected string OriginalValue;

    /// <summary>
    ///     The value of this object.
    /// </summary>
    protected string Value = NOTHING;

    /// <summary>
    ///     constructors
    /// </summary>
    /// <summary>
    ///     Constructs an empty  PdfString -object.
    /// </summary>
    public PdfString() : base(STRING)
    {
    }

    /// <summary>
    ///     Constructs a  PdfString -object.
    /// </summary>
    /// <param name="value">the content of the string</param>
    public PdfString(string value) : base(STRING) => Value = value;

    /// <summary>
    ///     Constructs a  PdfString -object.
    /// </summary>
    /// <param name="value">the content of the string</param>
    /// <param name="encoding">an encoding</param>
    public PdfString(string value, string encoding) : base(STRING)
    {
        Value = value;
        this.encoding = encoding;
    }

    /// <summary>
    ///     Constructs a  PdfString -object.
    /// </summary>
    /// <param name="bytes">an array of  byte </param>
    public PdfString(byte[] bytes) : base(STRING)
    {
        Value = PdfEncodings.ConvertToString(bytes, null);
        encoding = NOTHING;
    }

    /// <summary>
    ///     methods overriding some methods in PdfObject
    /// </summary>
    /// <summary>
    ///     Returns the PDF representation of this  PdfString .
    /// </summary>
    /// <returns>an array of  byte s</returns>

    public string Encoding => encoding;

    public override byte[] GetBytes()
    {
        if (Bytes == null)
        {
            if (encoding != null && encoding.Equals(TEXT_UNICODE, StringComparison.Ordinal) &&
                PdfEncodings.IsPdfDocEncoding(Value))
            {
                Bytes = PdfEncodings.ConvertToBytes(Value, TEXT_PDFDOCENCODING);
            }
            else
            {
                Bytes = PdfEncodings.ConvertToBytes(Value, encoding);
            }
        }

        return Bytes;
    }

    public byte[] GetOriginalBytes()
    {
        if (OriginalValue == null)
        {
            return GetBytes();
        }

        return PdfEncodings.ConvertToBytes(OriginalValue, null);
    }

    public bool IsHexWriting() => HexWriting;

    public PdfString SetHexWriting(bool hexWriting)
    {
        HexWriting = hexWriting;
        return this;
    }

    public override void ToPdf(PdfWriter writer, Stream os)
    {
        if (os == null)
        {
            throw new ArgumentNullException(nameof(os));
        }

        var b = GetBytes();
        PdfEncryption crypto = null;
        if (writer != null)
        {
            crypto = writer.Encryption;
        }

        if (crypto != null && !crypto.IsEmbeddedFilesOnly())
        {
            b = crypto.EncryptByteArray(b);
        }

        if (HexWriting)
        {
            var buf = new ByteBuffer();
            buf.Append('<');
            var len = b.Length;
            for (var k = 0; k < len; ++k)
            {
                buf.AppendHex(b[k]);
            }

            buf.Append('>');
            os.Write(buf.ToByteArray(), 0, buf.Size);
        }
        else
        {
            b = PdfContentByte.EscapeString(b);
            os.Write(b, 0, b.Length);
        }
    }

    /// <summary>
    ///     Returns the  string  value of the  PdfString -object.
    /// </summary>
    /// <returns>a  string </returns>
    public override string ToString() => Value;

    /// <summary>
    ///     other methods
    /// </summary>
    /// <summary>
    ///     Gets the encoding of this string.
    /// </summary>
    /// <returns>a  string </returns>
    public string ToUnicodeString()
    {
        if (!string.IsNullOrEmpty(encoding))
        {
            return Value;
        }

        GetBytes();
        if (Bytes.Length >= 2 && Bytes[0] == 254 && Bytes[1] == 255)
        {
            return PdfEncodings.ConvertToString(Bytes, TEXT_UNICODE);
        }

        return PdfEncodings.ConvertToString(Bytes, TEXT_PDFDOCENCODING);
    }

    internal void Decrypt(PdfReader reader)
    {
        var decrypt = reader.Decrypt;
        if (decrypt != null)
        {
            OriginalValue = Value;
            decrypt.SetHashKey(ObjNum, ObjGen);
            Bytes = PdfEncodings.ConvertToBytes(Value, null);
            Bytes = decrypt.DecryptByteArray(Bytes);
            Value = PdfEncodings.ConvertToString(Bytes, null);
        }
    }

    internal void SetObjNum(int objNum, int objGen)
    {
        ObjNum = objNum;
        ObjGen = objGen;
    }
}