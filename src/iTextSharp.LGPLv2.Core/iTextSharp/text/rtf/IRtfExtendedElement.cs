namespace iTextSharp.text.rtf;

/// <summary>
///     The RtfExtendedElement interface is to be used for elements that also
///     write data into the definition part of the rtf document
///     @author Mark Hall (Mark.Hall@mail.room3b.eu)
/// </summary>
public interface IRtfExtendedElement : IRtfBasicElement
{
    /// <summary>
    ///     Write the definition part of the element
    /// </summary>
    /// <param name="outp">The  OutputStream  to write the element definition to</param>
    void WriteDefinition(Stream outp);
}