using iTextSharp.text.rtf.document;

namespace iTextSharp.text.rtf.field;

/// <summary>
///     The RtfTotalPageNumber provides the total number of pages field in rtf documents.
///     @author Jose Hurtado (jose.hurtado@gft.com)
///     @author Mark Hall (Mark.Hall@mail.room3b.eu)
/// </summary>
public class RtfTotalPageNumber : RtfField
{
    /// <summary>
    ///     Constant for Arabic total page numbers.
    /// </summary>
    private static readonly byte[] _arabicTotalPages = DocWriter.GetIsoBytes("NUMPAGES \\\\* Arabic");

    /// <summary>
    ///     Constructs a RtfTotalPageNumber. This can be added anywhere to add a total number of pages field.
    /// </summary>
    public RtfTotalPageNumber() : base(null)
    {
    }

    /// <summary>
    ///     Constructs a RtfTotalPageNumber with a specified Font. This can be added anywhere
    ///     to add a total number of pages field.
    /// </summary>
    /// <param name="font"></param>
    public RtfTotalPageNumber(Font font) : base(null, font)
    {
    }

    /// <summary>
    ///     Constructs a RtfTotalPageNumber object.
    /// </summary>
    /// <param name="doc">The RtfDocument this RtfTotalPageNumber belongs to</param>
    public RtfTotalPageNumber(RtfDocument doc) : base(doc)
    {
    }

    /// <summary>
    ///     Constructs a RtfTotalPageNumber object with a specific font.
    /// </summary>
    /// <param name="doc">The RtfDocument this RtfTotalPageNumber belongs to</param>
    /// <param name="font">The Font to use</param>
    public RtfTotalPageNumber(RtfDocument doc, Font font) : base(doc, font)
    {
    }

    /// <summary>
    ///     Writes the field NUMPAGES instruction with Arabic format: "NUMPAGES \\\\* Arabic".
    /// </summary>
    protected override void WriteFieldInstContent(Stream oupt)
    {
        if (oupt == null)
        {
            throw new ArgumentNullException(nameof(oupt));
        }

        oupt.Write(_arabicTotalPages, 0, _arabicTotalPages.Length);
    }

    /// <summary>
    ///     Writes the field result content "1"
    /// </summary>
    protected override void WriteFieldResultContent(Stream oupt)
    {
        if (oupt == null)
        {
            throw new ArgumentNullException(nameof(oupt));
        }

        byte[] t = { (byte)'1' };
        oupt.Write(t, 0, t.Length);
    }
}