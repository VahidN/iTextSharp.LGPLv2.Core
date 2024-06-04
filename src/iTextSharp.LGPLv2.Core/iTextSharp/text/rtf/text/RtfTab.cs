namespace iTextSharp.text.rtf.text;

/// <summary>
///     The RtfTab encapsulates a tab position and tab type in a paragraph.
///     To add tabs to a paragraph construct new RtfTab objects with the desired
///     tab position and alignment and then add them to the paragraph. In the actual
///     text the tabs are then defined as standard \t characters.
///     RtfTab tab = new RtfTab(300, RtfTab.TAB_LEFT_ALIGN);
///     Paragraph para = new Paragraph();
///     para.Add(tab);
///     para.Add("This paragraph has a\ttab defined.");
///     @version $Revision: 1.5 $
///     @author Mark Hall (Mark.Hall@mail.room3b.eu)
/// </summary>
public class RtfTab : RtfAddableElement
{
    /// <summary>
    ///     A tab where the text is centre aligned.
    /// </summary>
    public const int TAB_CENTER_ALIGN = 1;

    /// <summary>
    ///     A tab where the text is aligned on the decimal character. Which
    ///     character that is depends on the language settings of the viewer.
    /// </summary>
    public const int TAB_DECIMAL_ALIGN = 3;

    /// <summary>
    ///     A tab where the text is left aligned.
    /// </summary>
    public const int TAB_LEFT_ALIGN = 0;

    /// <summary>
    ///     A tab where the text is right aligned.
    /// </summary>
    public const int TAB_RIGHT_ALIGN = 2;

    /// <summary>
    ///     The tab position in twips.
    /// </summary>
    private readonly int _position;

    /// <summary>
    ///     The tab alignment.
    /// </summary>
    private readonly int _type = TAB_LEFT_ALIGN;

    /// <summary>
    ///     Constructs a new RtfTab with the given position and type. The position
    ///     is in standard iText points. The type is one of the tab alignment
    ///     constants defined in the RtfTab.
    /// </summary>
    /// <param name="position">The position of the tab in points.</param>
    /// <param name="type">The tab type constant.</param>
    public RtfTab(float position, int type)
    {
        _position = (int)Math.Round(position * RtfElement.TWIPS_FACTOR);
        switch (type)
        {
            case TAB_LEFT_ALIGN:
                _type = TAB_LEFT_ALIGN;
                break;
            case TAB_CENTER_ALIGN:
                _type = TAB_CENTER_ALIGN;
                break;
            case TAB_RIGHT_ALIGN:
                _type = TAB_RIGHT_ALIGN;
                break;
            case TAB_DECIMAL_ALIGN:
                _type = TAB_DECIMAL_ALIGN;
                break;
            default:
                _type = TAB_LEFT_ALIGN;
                break;
        }
    }

    /// <summary>
    ///     Writes the tab settings.
    /// </summary>
    public override void WriteContent(Stream outp)
    {
        if (outp == null)
        {
            throw new ArgumentNullException(nameof(outp));
        }

        byte[] t;
        switch (_type)
        {
            case TAB_CENTER_ALIGN:
                outp.Write(t = DocWriter.GetIsoBytes("\\tqc"), 0, t.Length);
                break;
            case TAB_RIGHT_ALIGN:
                outp.Write(t = DocWriter.GetIsoBytes("\\tqr"), 0, t.Length);
                break;
            case TAB_DECIMAL_ALIGN:
                outp.Write(t = DocWriter.GetIsoBytes("\\tqdec"), 0, t.Length);
                break;
        }

        outp.Write(t = DocWriter.GetIsoBytes("\\tx"), 0, t.Length);
        outp.Write(t = IntToByteArray(_position), 0, t.Length);
    }
}