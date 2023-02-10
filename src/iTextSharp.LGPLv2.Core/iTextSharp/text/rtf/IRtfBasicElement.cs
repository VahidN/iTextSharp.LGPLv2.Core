using iTextSharp.text.rtf.document;

namespace iTextSharp.text.rtf;

public interface IRtfBasicElement : IRtfElementInterface
{
    /// <summary>
    ///     Writes the element content to the given output stream.
    /// </summary>
    /// <param name="outp">The  OutputStream  to write the content to</param>
    void WriteContent(Stream outp);

    /// <summary>
    ///     Sets the RtfDocument this RtfElement belongs to
    /// </summary>
    /// <param name="doc">The @link{com.lowagie.text.rtf.document.RtfDocument} this  RtfElement  belongs to</param>
    void SetRtfDocument(RtfDocument doc);

    /// <summary>
    ///     Sets whether this IRtfBasicElement is in a table
    /// </summary>
    /// <param name="inTable">Whether this IRtfBasicElement is in a table</param>
    void SetInTable(bool inTable);

    /// <summary>
    ///     Sets whether this IRtfBasicElement is in a header
    /// </summary>
    /// <param name="inHeader">Whether this IRtfBasicElement is in a header</param>
    void SetInHeader(bool inHeader);
}