using iTextSharp.text.rtf.document;
using iTextSharp.text.rtf.text;

namespace iTextSharp.text.rtf.field;

/// <summary>
///     @version $Version:$
///     @author Mark Hall (Mark.Hall@mail.room3b.eu)
/// </summary>
public class RtfAnchor : RtfField
{
    /// <summary>
    ///     Constant for a hyperlink
    /// </summary>
    private static readonly byte[] _hyperlink = DocWriter.GetIsoBytes("HYPERLINK");

    /// <summary>
    ///     The RtfPhrase to display for the url
    /// </summary>
    private readonly RtfPhrase _content;

    /// <summary>
    ///     The url of this RtfAnchor
    /// </summary>
    private readonly string _url = "";

    /// <summary>
    ///     Constructs a RtfAnchor based on a RtfField
    /// </summary>
    /// <param name="doc">The RtfDocument this RtfAnchor belongs to</param>
    /// <param name="anchor">The Anchor this RtfAnchor is based on</param>
    public RtfAnchor(RtfDocument doc, Anchor anchor) : base(doc)
    {
        if (anchor == null)
        {
            throw new ArgumentNullException(nameof(anchor));
        }

        _url = anchor.Reference;
        _content = new RtfPhrase(doc, anchor);
    }

    /// <summary>
    ///     Write the field instructions for this RtfAnchor. Sets the field
    ///     type to HYPERLINK and then writes the url.
    ///     @throws IOException
    /// </summary>
    /// <returns>The field instructions for this RtfAnchor</returns>
    protected override void WriteFieldInstContent(Stream oupt)
    {
        if (oupt == null)
        {
            throw new ArgumentNullException(nameof(oupt));
        }

        oupt.Write(_hyperlink, 0, _hyperlink.Length);
        oupt.Write(Delimiter, 0, Delimiter.Length);
        Document.FilterSpecialChar(oupt, _url, true, true);
    }

    /// <summary>
    ///     Write the field result for this RtfAnchor. Writes the content
    ///     of the RtfPhrase.
    /// </summary>
    protected override void WriteFieldResultContent(Stream oupt)
    {
        if (oupt == null)
        {
            throw new ArgumentNullException(nameof(oupt));
        }

        _content.WriteContent(oupt);
    }
}