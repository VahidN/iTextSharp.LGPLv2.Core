namespace iTextSharp.text.pdf;

/// <summary>
///     PdfIndirectObject  is the Pdf indirect object.
///     An <I>indirect object</I> is an object that has been labeled so that it can be referenced by
///     other objects. Any type of  PdfObject  may be labeled as an indirect object.
///     An indirect object consists of an object identifier, a direct object, and the <B>endobj</B>
///     keyword. The <I>object identifier</I> consists of an integer <I>object number</I>, an integer
///     <I>generation number</I>, and the <B>obj</B> keyword.
///     This object is described in the 'Portable Document Format Reference Manual version 1.3'
///     section 4.10 (page 53).
///     @see        PdfObject
///     @see        PdfIndirectReference
/// </summary>
public class PdfIndirectObject
{
    /// <summary>
    ///     membervariables
    /// </summary>
    internal static readonly byte[] Endobj = DocWriter.GetIsoBytes("\nendobj\n");

    internal static readonly int Sizeobj;

    internal static readonly byte[] Startobj = DocWriter.GetIsoBytes(" obj\n");

    /// <summary>
    ///     the generation number
    /// </summary>
    protected int Generation;

    /// <summary>
    ///     The object number
    /// </summary>
    protected int Number;

    internal readonly PdfObject Objecti;

    internal readonly PdfWriter Writer;

    /// <summary>
    ///     constructors
    /// </summary>
    static PdfIndirectObject() => Sizeobj = Startobj.Length + Endobj.Length;

    /// <summary>
    ///     Constructs a  PdfIndirectObject .
    /// </summary>
    internal PdfIndirectObject(int number, PdfObject objecti, PdfWriter writer) : this(number, 0, objecti, writer)
    {
    }

    internal PdfIndirectObject(PdfIndirectReference refi, PdfObject objecti, PdfWriter writer) : this(refi.Number,
     refi.Generation, objecti, writer)
    {
    }

    /// <summary>
    ///     Constructs a  PdfIndirectObject .
    /// </summary>
    internal PdfIndirectObject(int number, int generation, PdfObject objecti, PdfWriter writer)
    {
        Writer = writer;
        Number = number;
        Generation = generation;
        Objecti = objecti;
        PdfEncryption crypto = null;
        if (writer != null)
        {
            crypto = writer.Encryption;
        }

        if (crypto != null)
        {
            crypto.SetHashKey(number, generation);
        }
    }

    /// <summary>
    ///     methods
    /// </summary>
    /// <summary>
    ///     Returns a  PdfIndirectReference  to this  PdfIndirectObject .
    /// </summary>
    /// <returns>a  PdfIndirectReference </returns>

    public PdfIndirectReference IndirectReference => new(Objecti.Type, Number, Generation);

    /// <summary>
    ///     Writes eficiently to a stream
    ///     @throws IOException on write error
    /// </summary>
    /// <param name="os">the stream to write to</param>
    internal void WriteTo(Stream os)
    {
        var tmp = DocWriter.GetIsoBytes(Number.ToString(CultureInfo.InvariantCulture));
        os.Write(tmp, 0, tmp.Length);
        os.WriteByte((byte)' ');
        tmp = DocWriter.GetIsoBytes(Generation.ToString(CultureInfo.InvariantCulture));
        os.Write(tmp, 0, tmp.Length);
        os.Write(Startobj, 0, Startobj.Length);
        Objecti.ToPdf(Writer, os);
        os.Write(Endobj, 0, Endobj.Length);
    }
}