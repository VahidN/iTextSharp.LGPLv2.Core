namespace iTextSharp.text.pdf;

/// <summary>
///     PdfObject  is the abstract baseclass of all PDF objects.
///     PDF supports seven basic types of objects: bools, numbers, strings, names,
///     arrays, dictionaries and streams. In addition, PDF provides a null object.
///     Objects may be labeled so that they can be referred to by other objects.
///     All these basic PDF objects are described in the 'Portable Document Format
///     Reference Manual version 1.3' Chapter 4 (pages 37-54).
///     @see        PdfNull
///     @see        Pdfbool
///     @see        PdfNumber
///     @see        PdfString
///     @see        PdfName
///     @see        PdfArray
///     @see        PdfDictionary
///     @see        PdfStream
///     @see        PdfIndirectReference
/// </summary>
public abstract class PdfObject
{
    /// <summary>
    ///     static membervariables (all the possible types of a PdfObject)
    /// </summary>
    /// <summary>
    ///     a possible type of  PdfObject
    /// </summary>
    public const int ARRAY = 5;

    /// <summary>
    ///     a possible type of  PdfObject
    /// </summary>
    public const int BOOLEAN = 1;

    /// <summary>
    ///     a possible type of  PdfObject
    /// </summary>
    public const int DICTIONARY = 6;

    /// <summary>
    ///     a possible type of  PdfObject
    /// </summary>
    public const int INDIRECT = 10;

    /// <summary>
    ///     a possible type of  PdfObject
    /// </summary>
    public const int NAME = 4;

    /// <summary>
    ///     This is an empty string used for the  PdfNull -object and for an empty  PdfString -object.
    /// </summary>
    public const string NOTHING = "";

    /// <summary>
    ///     a possible type of  PdfObject
    /// </summary>
    public const int NULL = 8;

    /// <summary>
    ///     a possible type of  PdfObject
    /// </summary>
    public const int NUMBER = 2;

    /// <summary>
    ///     a possible type of  PdfObject
    /// </summary>
    public const int STREAM = 7;

    /// <summary>
    ///     a possible type of  PdfObject
    /// </summary>
    public const int STRING = 3;

    /// <summary>
    ///     This is the default encoding to be used for converting strings into bytes and vice versa.
    ///     The default encoding is PdfDocEcoding.
    /// </summary>
    public const string TEXT_PDFDOCENCODING = "PDF";

    /// <summary>
    ///     This is the encoding to be used to output text in Unicode.
    /// </summary>
    public const string TEXT_UNICODE = "UnicodeBig";

    /// <summary>
    ///     membervariables
    /// </summary>
    /// <summary>
    ///     the content of this  PdfObject
    /// </summary>
    protected byte[] Bytes;

    /// <summary>
    ///     Holds value of property indRef.
    /// </summary>
    protected PrIndirectReference indRef;

    /// <summary>
    ///     the type of this  PdfObject
    /// </summary>
    protected int type;

    /// <summary>
    ///     constructors
    /// </summary>
    /// <summary>
    ///     Constructs a  PdfObject  of a certain <VAR>type</VAR> without any <VAR>content</VAR>.
    /// </summary>
    /// <param name="type">type of the new  PdfObject </param>
    protected PdfObject(int type) => this.type = type;

    /// <summary>
    ///     Constructs a  PdfObject  of a certain <VAR>type</VAR> with a certain <VAR>content</VAR>.
    /// </summary>
    /// <param name="type">type of the new  PdfObject </param>
    /// <param name="content">content of the new  PdfObject  as a  String .</param>
    protected PdfObject(int type, string content)
    {
        this.type = type;
        Bytes = PdfEncodings.ConvertToBytes(content, null);
    }

    /// <summary>
    ///     Constructs a  PdfObject  of a certain <VAR>type</VAR> with a certain <VAR>content</VAR>.
    /// </summary>
    /// <param name="type">type of the new  PdfObject </param>
    /// <param name="bytes">content of the new  PdfObject  as an array of  byte .</param>
    protected PdfObject(int type, byte[] bytes)
    {
        Bytes = bytes;
        this.type = type;
    }

    public PrIndirectReference IndRef
    {
        get => indRef;
        set => indRef = value;
    }

    public int Length => ToString().Length;

    public int Type => type;

    protected string Content
    {
        set => Bytes = PdfEncodings.ConvertToBytes(value, null);
    }

    /// <summary>
    ///     Can this object be in an object stream?
    /// </summary>
    /// <returns>true if this object can be in an object stream.</returns>
    public bool CanBeInObjStm()
    {
        switch (type)
        {
            case NULL:
            case BOOLEAN:
            case NUMBER:
            case STRING:
            case NAME:
            case ARRAY:
            case DICTIONARY:
                return true;
            case STREAM:
            case INDIRECT:
            default:
                return false;
        }
    }

    /// <summary>
    ///     Gets the presentation of this object in a byte array
    /// </summary>
    /// <returns>a byte array</returns>
    public virtual byte[] GetBytes() => Bytes;

    public bool IsArray() => type == ARRAY;

    public bool IsBoolean() => type == BOOLEAN;

    public bool IsDictionary() => type == DICTIONARY;

    /// <summary>
    ///     Checks if this is an indirect object.
    /// </summary>
    /// <returns>true if this is an indirect object</returns>
    public bool IsIndirect() => type == INDIRECT;

    public bool IsName() => type == NAME;

    public bool IsNull() => type == NULL;

    public bool IsNumber() => type == NUMBER;

    public bool IsStream() => type == STREAM;

    public bool IsString() => type == STRING;

    public virtual void ToPdf(PdfWriter writer, Stream os)
    {
        if (os == null)
        {
            throw new ArgumentNullException(nameof(os));
        }

        if (Bytes != null)
        {
            os.Write(Bytes, 0, Bytes.Length);
        }
    }

    /// <summary>
    ///     Returns the length of the PDF representation of the  PdfObject .
    ///     In some cases, namely for  PdfString  and  PdfStream ,
    ///     this method differs from the method  length  because  length
    ///     returns the length of the actual content of the  PdfObject .
    ///     Remark: the actual content of an object is in most cases identical to its representation.
    ///     The following statement is always true: Length() &gt;= PdfLength().
    /// </summary>
    /// <returns>a length</returns>
    /// <summary>
    ///     public int PdfLength() {
    /// </summary>
    /// <summary>
    ///     return ToPdf(null).length;
    /// </summary>
    /// <summary>
    ///     }
    /// </summary>
    /// <summary>
    ///     Returns the  String -representation of this  PdfObject .
    /// </summary>
    /// <returns>a  String </returns>
    public override string ToString()
    {
        if (Bytes == null)
        {
            return "";
        }

        return PdfEncodings.ConvertToString(Bytes, null);
    }
}