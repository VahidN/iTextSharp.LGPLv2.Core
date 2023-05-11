namespace iTextSharp.text.rtf.field;

/// <summary>
///     The RtfTableOfContents together with multiple RtfTOCEntry objects generates a table
///     of contents. The table of contents will display no entries in the viewing program
///     and the user will have to update it first. A text to inform the user of this is
///     displayed instead.
///     @version $Version:$
///     @author Mark Hall (Mark.Hall@mail.room3b.eu)
///     @author <a href="mailto:Steffen.Stundzig@smb-tec.com">Steffen.Stundzig@smb-tec.com</a>
/// </summary>
public class RtfTableOfContents : RtfField
{
    /// <summary>
    ///     field inst content
    /// </summary>
    private const string FieldInst = "TOC \\\\f \\\\h \\\\u \\\\o \"1-5\" ";

    /// <summary>
    ///     The default text to display
    /// </summary>
    private readonly string _defaultText = "Table of Contents - Click to update";

    /// <summary>
    ///     Constructs a RtfTableOfContents. The default text is the text that is displayed
    ///     before the user updates the table of contents
    /// </summary>
    /// <param name="defaultText">The default text to display</param>
    public RtfTableOfContents(string defaultText) : base(null, new Font()) => _defaultText = defaultText;

    /// <summary>
    ///     Writes the field instruction content
    /// </summary>
    protected override void WriteFieldInstContent(Stream oupt)
    {
        if (oupt == null)
        {
            throw new ArgumentNullException(nameof(oupt));
        }

        var t = DocWriter.GetIsoBytes(FieldInst);
        oupt.Write(t, 0, t.Length);
    }

    /// <summary>
    ///     Writes the field result content
    /// </summary>
    protected override void WriteFieldResultContent(Stream oupt)
    {
        if (oupt == null)
        {
            throw new ArgumentNullException(nameof(oupt));
        }

        Document.FilterSpecialChar(oupt, _defaultText, true, true);
    }
}