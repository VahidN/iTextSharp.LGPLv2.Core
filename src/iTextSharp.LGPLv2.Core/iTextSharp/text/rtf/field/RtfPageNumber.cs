using iTextSharp.text.rtf.document;

namespace iTextSharp.text.rtf.field;

/// <summary>
///     The RtfPageNumber provides the page number field in rtf documents.
///     @version $Revision: 1.4 $
///     @author Mark Hall (Mark.Hall@mail.room3b.eu)
///     @author <a href="mailto:Steffen.Stundzig@smb-tec.com">Steffen.Stundzig@smb-tec.com</a>
/// </summary>
public class RtfPageNumber : RtfField
{
    /// <summary>
    ///     Constant for the page number
    /// </summary>
    private static readonly byte[] _pageNumber = DocWriter.GetIsoBytes("PAGE");

    /// <summary>
    ///     Constructs a RtfPageNumber. This can be added anywhere to add a page number field.
    /// </summary>
    public RtfPageNumber() : base(null)
    {
    }

    /// <summary>
    ///     Constructs a RtfPageNumber with a specified Font. This can be added anywhere to
    ///     add a page number field.
    /// </summary>
    /// <param name="font"></param>
    public RtfPageNumber(Font font) : base(null, font)
    {
    }

    /// <summary>
    ///     Constructs a RtfPageNumber object.
    /// </summary>
    /// <param name="doc">The RtfDocument this RtfPageNumber belongs to</param>
    public RtfPageNumber(RtfDocument doc) : base(doc)
    {
    }

    /// <summary>
    ///     Constructs a RtfPageNumber object with a specific font.
    /// </summary>
    /// <param name="doc">The RtfDocument this RtfPageNumber belongs to</param>
    /// <param name="font">The Font to use</param>
    public RtfPageNumber(RtfDocument doc, Font font) : base(doc, font)
    {
    }

    /// <summary>
    ///     Writes the field instruction content
    ///     @
    /// </summary>
    protected override void WriteFieldInstContent(Stream oupt)
    {
        if (oupt == null)
        {
            throw new ArgumentNullException(nameof(oupt));
        }

        oupt.Write(_pageNumber, 0, _pageNumber.Length);
    }

    /// <summary>
    ///     Writes the field result content
    ///     @
    /// </summary>
    protected override void WriteFieldResultContent(Stream oupt)
    {
    }
}