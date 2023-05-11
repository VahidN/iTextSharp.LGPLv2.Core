using iTextSharp.text.rtf.document;

namespace iTextSharp.text.rtf.style;

/// <summary>
///     The RtfFontList stores the list of fonts used in the rtf document. It also
///     has methods for writing this list to the document
///     @author Mark Hall (Mark.Hall@mail.room3b.eu)
/// </summary>
public class RtfFontList : RtfElement, IRtfExtendedElement
{
    /// <summary>
    ///     Constant for the font number
    /// </summary>
    public static readonly byte[] FontNumber = DocWriter.GetIsoBytes("\\f");

    /// <summary>
    ///     Constant for the default font
    /// </summary>
    private static readonly byte[] _defaultFont = DocWriter.GetIsoBytes("\\deff");

    /// <summary>
    ///     Constant for the font table
    /// </summary>
    private static readonly byte[] _fontTable = DocWriter.GetIsoBytes("\\fonttbl");

    /// <summary>
    ///     The list of fonts
    /// </summary>
    private readonly List<RtfFont> _fontList = new();

    /// <summary>
    ///     Creates a RtfFontList
    /// </summary>
    /// <param name="doc">The RtfDocument this RtfFontList belongs to</param>
    public RtfFontList(RtfDocument doc) : base(doc)
    {
        _fontList.Add(new RtfFont(Document, 0));
    }

    /// <summary>
    ///     unused
    /// </summary>
    public override void WriteContent(Stream outp)
    {
    }

    /// <summary>
    ///     Writes the definition of the font list
    /// </summary>
    public virtual void WriteDefinition(Stream outp)
    {
        if (outp == null)
        {
            throw new ArgumentNullException(nameof(outp));
        }

        byte[] t;
        outp.Write(_defaultFont, 0, _defaultFont.Length);
        outp.Write(t = IntToByteArray(0), 0, t.Length);
        outp.Write(OpenGroup, 0, OpenGroup.Length);
        outp.Write(_fontTable, 0, _fontTable.Length);
        for (var i = 0; i < _fontList.Count; i++)
        {
            outp.Write(OpenGroup, 0, OpenGroup.Length);
            outp.Write(FontNumber, 0, FontNumber.Length);
            outp.Write(t = IntToByteArray(i), 0, t.Length);
            var rf = _fontList[i];
            rf.WriteDefinition(outp);
            outp.Write(CommaDelimiter, 0, CommaDelimiter.Length);
            outp.Write(CloseGroup, 0, CloseGroup.Length);
        }

        outp.Write(CloseGroup, 0, CloseGroup.Length);
        Document.OutputDebugLinebreak(outp);
    }

    /// <summary>
    ///     Gets the index of the font in the list of fonts. If the font does not
    ///     exist in the list, it is added.
    /// </summary>
    /// <param name="font">The font to get the id for</param>
    /// <returns>The index of the font</returns>
    public int GetFontNumber(RtfFont font)
    {
        if (font is RtfParagraphStyle)
        {
            font = new RtfFont(Document, (RtfParagraphStyle)font);
        }

        var fontIndex = -1;
        for (var i = 0; i < _fontList.Count; i++)
        {
            if (_fontList[i].Equals(font))
            {
                fontIndex = i;
            }
        }

        if (fontIndex == -1)
        {
            fontIndex = _fontList.Count;
            _fontList.Add(font);
        }

        return fontIndex;
    }
}