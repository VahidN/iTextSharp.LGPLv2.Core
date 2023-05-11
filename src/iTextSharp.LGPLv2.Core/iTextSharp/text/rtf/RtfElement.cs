using iTextSharp.text.rtf.document;

namespace iTextSharp.text.rtf;

/// <summary>
///     RtfElement is the base class for all RTF Element classes
///     Version: $Id: RtfElement.cs,v 1.5 2008/05/16 19:30:14 psoares33 Exp $
///     @author Mark Hall (Mark.Hall@mail.room3b.eu)
/// </summary>
public abstract class RtfElement : IRtfBasicElement
{
    /// <summary>
    ///     The factor to use for translating from iText to rtf measurments
    /// </summary>
    public const double TWIPS_FACTOR = 20;

    /// <summary>
    ///     Constant for the end of an rtf group
    /// </summary>
    public static readonly byte[] CloseGroup = { (byte)'}' };

    /// <summary>
    ///     Constant for a comma delimiter in rtf
    /// </summary>
    public static readonly byte[] CommaDelimiter = { (byte)';' };

    /// <summary>
    ///     Constant for a delimiter in rtf
    /// </summary>
    public static readonly byte[] Delimiter = { (byte)' ' };

    /// <summary>
    ///     Constant for the beginning of a rtf group
    /// </summary>
    public static readonly byte[] OpenGroup = { (byte)'{' };

    /// <summary>
    ///     The RtfDocument this RtfElement belongs to
    /// </summary>
    protected RtfDocument Document;

    /// <summary>
    ///     Whether this RtfElement is in a header
    /// </summary>
    protected bool InHeader;

    /// <summary>
    ///     Whether this RtfElement is in a table
    /// </summary>
    public bool InTable;

    /// <summary>
    ///     Constructs a RtfElement belonging to the specified RtfDocument.
    /// </summary>
    /// <param name="doc">The RtfDocument this RtfElement belongs to</param>
    protected RtfElement(RtfDocument doc) => Document = doc;

    /// <summary>
    ///     Sets whether this RtfElement is in a header
    /// </summary>
    /// <param name="inHeader"> True  if this RtfElement is in a header,  false  otherwise</param>
    public virtual void SetInHeader(bool inHeader)
    {
        InHeader = inHeader;
    }

    /// <summary>
    ///     Sets whether this RtfElement is in a table
    /// </summary>
    /// <param name="inTable"> True  if this RtfElement is in a table,  false  otherwise</param>
    public virtual void SetInTable(bool inTable)
    {
        InTable = inTable;
    }

    /// <summary>
    ///     Sets the RtfDocument this RtfElement belongs to
    /// </summary>
    /// <param name="doc">The RtfDocument to use</param>
    public virtual void SetRtfDocument(RtfDocument doc)
    {
        Document = doc;
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
    ///     Gets whether this RtfElement is in a table
    /// </summary>
    /// <returns>Whether this RtfElement is in a table</returns>
    public virtual bool IsInTable() => InTable;
}