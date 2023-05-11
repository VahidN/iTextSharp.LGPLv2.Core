using iTextSharp.text.rtf.document;

namespace iTextSharp.text.rtf.text;

/// <summary>
///     The RtfNewPage creates a new page. INTERNAL CLASS
///     @version $Version:$
///     @author Mark Hall (Mark.Hall@mail.room3b.eu)
/// </summary>
public class RtfNewPage : RtfElement
{
    /// <summary>
    ///     Constant for a new page
    /// </summary>
    public static readonly byte[] NewPage = DocWriter.GetIsoBytes("\\page");

    /// <summary>
    ///     Constructs a RtfNewPage
    /// </summary>
    /// <param name="doc">The RtfDocument this RtfNewPage belongs to</param>
    public RtfNewPage(RtfDocument doc) : base(doc)
    {
    }

    /// <summary>
    ///     Writes a new page
    /// </summary>
    public override void WriteContent(Stream outp)
    {
        if (outp == null)
        {
            throw new ArgumentNullException(nameof(outp));
        }

        outp.Write(NewPage, 0, NewPage.Length);
        outp.Write(RtfPhrase.ParagraphDefaults, 0, RtfPhrase.ParagraphDefaults.Length);
    }
}