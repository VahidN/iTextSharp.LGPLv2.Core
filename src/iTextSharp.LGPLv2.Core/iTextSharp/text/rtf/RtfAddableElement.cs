using iTextSharp.text.rtf.document;

namespace iTextSharp.text.rtf;

/// <summary>
///     The RtfAddableElement is the superclass for all rtf specific elements
///     that need to be added to an iText document. It is an extension of Chunk
///     and it also implements RtfBasicElement. It is an abstract class thus it
///     cannot be instantiated itself and has to be subclassed to be used.
///     @version $Revision: 1.6 $
///     @author Mark Hall (Mark.Hall@mail.room3b.eu)
/// </summary>
public abstract class RtfAddableElement : Chunk, IRtfBasicElement
{
    /// <summary>
    ///     The RtfDocument this RtfAddableElement belongs to.
    /// </summary>
    protected RtfDocument Doc;

    /// <summary>
    ///     Whether this RtfAddableElement is contained in a header.
    /// </summary>
    protected bool InHeader;

    /// <summary>
    ///     Whether this RtfAddableElement is contained in a table.
    /// </summary>
    protected bool InTable;

    /// <summary>
    ///     Constructs a new RtfAddableElement. The Chunk content is
    ///     set to an empty string and the font to the default Font().
    /// </summary>
    protected RtfAddableElement() : base("", new Font())
    {
    }

    /// <summary>
    ///     Sets whether this RtfAddableElement is contained in a header/footer.
    /// </summary>
    public void SetInHeader(bool inHeader)
    {
        InHeader = inHeader;
    }

    /// <summary>
    ///     Sets whether this RtfAddableElement is contained in a table.
    /// </summary>
    public void SetInTable(bool inTable)
    {
        InTable = inTable;
    }

    /// <summary>
    ///     Sets the RtfDocument this RtfAddableElement belongs to.
    /// </summary>
    public void SetRtfDocument(RtfDocument doc)
    {
        Doc = doc;
    }

    /// <summary>
    ///     Writes the element content to the given output stream.
    /// </summary>
    public abstract void WriteContent(Stream outp);

    /// <summary>
    ///     Transforms an integer into its String representation and then returns the bytes
    ///     of that string.
    /// </summary>
    /// <param name="i">The integer to convert</param>
    /// <returns>A byte array representing the integer</returns>
    public static byte[] IntToByteArray(int i) => DocWriter.GetIsoBytes(i.ToString(CultureInfo.InvariantCulture));

    /// <summary>
    ///     RtfAddableElement subclasses are never assumed to be empty.
    /// </summary>
    public override bool IsEmpty() => false;
}