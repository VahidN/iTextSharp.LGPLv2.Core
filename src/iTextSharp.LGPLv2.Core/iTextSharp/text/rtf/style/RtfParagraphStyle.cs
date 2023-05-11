using iTextSharp.text.rtf.document;
using iTextSharp.text.rtf.text;

namespace iTextSharp.text.rtf.style;

/// <summary>
///     The RtfParagraphStyle stores all style/formatting attributes of a RtfParagraph.
///     Additionally it also supports the style name system available in RTF. The RtfParagraphStyle
///     is a Font and can thus be used as such. To use the stylesheet functionality
///     it needs to be set as the font of a Paragraph. Otherwise it will work like a
///     RtfFont. It also supports inheritance of styles.
///     @version $Revision: 1.8 $
///     @author Mark Hall (Mark.Hall@mail.room3b.eu)
/// </summary>
public class RtfParagraphStyle : RtfFont
{
    /// <summary>
    ///     The alignment has been modified.
    /// </summary>
    private const int ModifiedAlignment = 1;

    /// <summary>
    ///     The font colour has been modified.
    /// </summary>
    private const int ModifiedFontColor = 256;

    /// <summary>
    ///     The font name has been modified.
    /// </summary>
    private const int ModifiedFontName = 32;

    /// <summary>
    ///     The font style has been modified.
    /// </summary>
    private const int ModifiedFontSize = 64;

    /// <summary>
    ///     The font size has been modified.
    /// </summary>
    private const int ModifiedFontStyle = 128;

    /// <summary>
    ///     The left indentation has been modified.
    /// </summary>
    private const int ModifiedIndentLeft = 2;

    /// <summary>
    ///     The right indentation has been modified.
    /// </summary>
    private const int ModifiedIndentRight = 4;

    /// <summary>
    ///     The paragraph keep together setting has been modified.
    /// </summary>
    private const int ModifiedKeepTogether = 1024;

    /// <summary>
    ///     The paragraph keep together with next setting has been modified.
    /// </summary>
    private const int ModifiedKeepTogetherWithNext = 2048;

    /// <summary>
    ///     The line leading has been modified.
    /// </summary>
    private const int ModifiedLineLeading = 512;

    /// <summary>
    ///     No modification has taken place when compared to the RtfParagraphStyle this RtfParagraphStyle
    ///     is based on. These modification markers are used to determine what needs to be
    ///     inherited and what not from the parent RtfParagraphStyle.
    /// </summary>
    private const int ModifiedNone = 0;

    /// <summary>
    ///     The spacing after a paragraph has been modified.
    /// </summary>
    private const int ModifiedSpacingAfter = 16;

    /// <summary>
    ///     The spacing before a paragraph has been modified.
    /// </summary>
    private const int ModifiedSpacingBefore = 8;

    /// <summary>
    ///     Constant for center alignment
    /// </summary>
    public static readonly byte[] AlignCenter = DocWriter.GetIsoBytes("\\qc");

    /// <summary>
    ///     Constant for justified alignment
    /// </summary>
    public static readonly byte[] AlignJustify = DocWriter.GetIsoBytes("\\qj");

    /// <summary>
    ///     Constant for left alignment
    /// </summary>
    public static readonly byte[] AlignLeft = DocWriter.GetIsoBytes("\\ql");

    /// <summary>
    ///     Constant for right alignment
    /// </summary>
    public static readonly byte[] AlignRight = DocWriter.GetIsoBytes("\\qr");

    /// <summary>
    ///     Constant for the first line indentation
    /// </summary>
    public static readonly byte[] FirstLineIndent = DocWriter.GetIsoBytes("\\fi");

    /// <summary>
    ///     Constant for left indentation
    /// </summary>
    public static readonly byte[] IndentLeft = DocWriter.GetIsoBytes("\\li");

    /// <summary>
    ///     Constant for right indentation
    /// </summary>
    public static readonly byte[] IndentRight = DocWriter.GetIsoBytes("\\ri");

    /// <summary>
    ///     Constant for keeping the paragraph together on one page
    /// </summary>
    public static readonly byte[] KeepTogether = DocWriter.GetIsoBytes("\\keep");

    /// <summary>
    ///     Constant for keeping the paragraph toghether with the next one on one page
    /// </summary>
    public static readonly byte[] KeepTogetherWithNext = DocWriter.GetIsoBytes("\\keepn");

    /// <summary>
    ///     Constant for the space after the paragraph.
    /// </summary>
    public static readonly byte[] SpacingAfter = DocWriter.GetIsoBytes("\\sa");

    /// <summary>
    ///     Constant for the space before the paragraph.
    /// </summary>
    public static readonly byte[] SpacingBefore = DocWriter.GetIsoBytes("\\sb");

    /// <summary>
    ///     The style for level 1 headings.
    /// </summary>
    public static readonly RtfParagraphStyle StyleHeading1 = new("heading 1", "Normal");

    /// <summary>
    ///     The style for level 2 headings.
    /// </summary>
    public static readonly RtfParagraphStyle StyleHeading2 = new("heading 2", "Normal");

    /// <summary>
    ///     The style for level 3 headings.
    /// </summary>
    public static readonly RtfParagraphStyle StyleHeading3 = new("heading 3", "Normal");

    /// <summary>
    ///     The NORMAL/STANDARD style.
    /// </summary>
    public static readonly RtfParagraphStyle StyleNormal = new("Normal", "Arial", 12, NORMAL, BaseColor.Black);

    /// <summary>
    ///     The name of the RtfParagraphStyle this RtfParagraphStyle is based on.
    /// </summary>
    private readonly string _basedOnName;

    /// <summary>
    ///     The name of this RtfParagraphStyle.
    /// </summary>
    private readonly string _styleName = "";

    /// <summary>
    ///     The alignment of the paragraph.
    /// </summary>
    private int _alignment = Element.ALIGN_LEFT;

    /// <summary>
    ///     The RtfParagraphStyle this RtfParagraphStyle is based on.
    /// </summary>
    private RtfParagraphStyle _baseStyle;

    /// <summary>
    ///     The indentation for the first line
    /// </summary>
    private int _firstLineIndent;

    /// <summary>
    ///     The left indentation of the paragraph.
    /// </summary>
    private int _indentLeft;

    /// <summary>
    ///     The right indentation of the paragraph.
    /// </summary>
    private int _indentRight;

    /// <summary>
    ///     Whether this RtfParagraph must stay on one page.
    /// </summary>
    private bool _keepTogether;

    /// <summary>
    ///     Whether this RtfParagraph must stay on the same page as the next paragraph.
    /// </summary>
    private bool _keepTogetherWithNext;

    /// <summary>
    ///     The line leading of the paragraph.
    /// </summary>
    private int _lineLeading;

    /// <summary>
    ///     Which properties have been modified when compared to the base style.
    /// </summary>
    private int _modified = ModifiedNone;

    /// <summary>
    ///     The spacing after a paragraph.
    /// </summary>
    private int _spacingAfter;

    /// <summary>
    ///     The spacing before a paragraph.
    /// </summary>
    private int _spacingBefore;

    /// <summary>
    ///     The number of this RtfParagraphStyle in the stylesheet list.
    /// </summary>
    private int _styleNumber = -1;

    /// <summary>
    ///     Initialises the properties of the styles.
    /// </summary>
    static RtfParagraphStyle()
    {
        StyleHeading1.Size = 16;
        StyleHeading1.SetStyle(BOLD);
        StyleHeading2.Size = 14;
        StyleHeading2.SetStyle(BOLDITALIC);
        StyleHeading3.Size = 13;
        StyleHeading3.SetStyle(BOLD);
    }

    /// <summary>
    ///     Constructs a new RtfParagraphStyle with the given attributes.
    /// </summary>
    /// <param name="styleName">The name of this RtfParagraphStyle.</param>
    /// <param name="fontName">The name of the font to use for this RtfParagraphStyle.</param>
    /// <param name="fontSize">The size of the font to use for this RtfParagraphStyle.</param>
    /// <param name="fontStyle">The style of the font to use for this RtfParagraphStyle.</param>
    /// <param name="fontColor">The colour of the font to use for this RtfParagraphStyle.</param>
    public RtfParagraphStyle(string styleName, string fontName, int fontSize, int fontStyle, BaseColor fontColor) :
        base(null, new RtfFont(fontName, fontSize, fontStyle, fontColor)) => _styleName = styleName;

    /// <summary>
    ///     Constructs a new RtfParagraphStyle that is based on an existing RtfParagraphStyle.
    /// </summary>
    /// <param name="styleName">The name of this RtfParagraphStyle.</param>
    /// <param name="basedOnName">The name of the RtfParagraphStyle this RtfParagraphStyle is based on.</param>
    public RtfParagraphStyle(string styleName, string basedOnName) : base(null, new Font())
    {
        _styleName = styleName;
        _basedOnName = basedOnName;
    }

    /// <summary>
    ///     Constructs a RtfParagraphStyle from another RtfParagraphStyle.
    ///     INTERNAL USE ONLY
    /// </summary>
    /// <param name="doc">The RtfDocument this RtfParagraphStyle belongs to.</param>
    /// <param name="style">The RtfParagraphStyle to copy settings from.</param>
    public RtfParagraphStyle(RtfDocument doc, RtfParagraphStyle style) : base(doc, style)
    {
        if (style == null)
        {
            throw new ArgumentNullException(nameof(style));
        }

        Document = doc;
        _styleName = style.GetStyleName();
        _alignment = style.GetAlignment();
        _firstLineIndent = (int)(style.GetFirstLineIndent() * RtfElement.TWIPS_FACTOR);
        _indentLeft = (int)(style.GetIndentLeft() * RtfElement.TWIPS_FACTOR);
        _indentRight = (int)(style.GetIndentRight() * RtfElement.TWIPS_FACTOR);
        _spacingBefore = (int)(style.GetSpacingBefore() * RtfElement.TWIPS_FACTOR);
        _spacingAfter = (int)(style.GetSpacingAfter() * RtfElement.TWIPS_FACTOR);
        _lineLeading = (int)(style.GetLineLeading() * RtfElement.TWIPS_FACTOR);
        _keepTogether = style.GetKeepTogether();
        _keepTogetherWithNext = style.GetKeepTogetherWithNext();
        _basedOnName = style._basedOnName;
        _modified = style._modified;
        _styleNumber = style.getStyleNumber();

        if (Document != null)
        {
            SetRtfDocument(Document);
        }
    }

    /// <summary>
    ///     Sets the font size of this RtfParagraphStyle.
    /// </summary>
    public override float Size
    {
        set
        {
            _modified = _modified | ModifiedFontSize;
            base.Size = value;
        }
    }

    /// <summary>
    ///     Tests whether two RtfParagraphStyles are equal. Equality
    ///     is determined via the name.
    /// </summary>
    public override bool Equals(object obj)
    {
        if (!(obj is RtfParagraphStyle))
        {
            return false;
        }

        var paragraphStyle = (RtfParagraphStyle)obj;
        var result = GetStyleName().Equals(paragraphStyle.GetStyleName(), StringComparison.Ordinal);
        return result;
    }

    /// <summary>
    ///     Gets the alignment of this RtfParagraphStyle.
    /// </summary>
    /// <returns>The alignment of this RtfParagraphStyle.</returns>
    public int GetAlignment() => _alignment;

    /// <summary>
    ///     Gets the name of the RtfParagraphStyle this RtfParagraphStyle is based on.
    /// </summary>
    /// <returns>The name of the base RtfParagraphStyle.</returns>
    public string GetBasedOnName() => _basedOnName;

    /// <summary>
    ///     Gets the first line indentation of this RtfParagraphStyle.
    /// </summary>
    /// <returns>The first line indentation of this RtfParagraphStyle.</returns>
    public int GetFirstLineIndent() => _firstLineIndent;

    /// <summary>
    ///     Gets the hash code of this RtfParagraphStyle.
    /// </summary>
    public override int GetHashCode() => _styleName.GetHashCode();

    /// <summary>
    ///     Gets the left indentation of this RtfParagraphStyle.
    /// </summary>
    /// <returns>The left indentation of this RtfParagraphStyle.</returns>
    public int GetIndentLeft() => _indentLeft;

    /// <summary>
    ///     Gets the right indentation of this RtfParagraphStyle.
    /// </summary>
    /// <returns>The right indentation of this RtfParagraphStyle.</returns>
    public int GetIndentRight() => _indentRight;

    /// <summary>
    ///     Gets whether the lines in the paragraph should be kept together in
    ///     this RtfParagraphStyle.
    /// </summary>
    /// <returns>Whether the lines in the paragraph should be kept together.</returns>
    public bool GetKeepTogether() => _keepTogether;

    /// <summary>
    ///     Gets whether the paragraph should be kept toggether with the next in
    ///     this RtfParagraphStyle.
    /// </summary>
    /// <returns>Whether the paragraph should be kept together with the next.</returns>
    public bool GetKeepTogetherWithNext() => _keepTogetherWithNext;

    /// <summary>
    ///     Gets the line leading of this RtfParagraphStyle.
    /// </summary>
    /// <returns>The line leading of this RtfParagraphStyle.</returns>
    public int GetLineLeading() => _lineLeading;

    /// <summary>
    ///     Gets the space after the paragraph of this RtfParagraphStyle.
    /// </summary>
    /// <returns>The space after the paragraph.</returns>
    public int GetSpacingAfter() => _spacingAfter;

    /// <summary>
    ///     Gets the space before the paragraph of this RtfParagraphStyle..
    /// </summary>
    /// <returns>The space before the paragraph.</returns>
    public int GetSpacingBefore() => _spacingBefore;

    /// <summary>
    ///     Gets the name of this RtfParagraphStyle.
    /// </summary>
    /// <returns>The name of this RtfParagraphStyle.</returns>
    public string GetStyleName() => _styleName;

    /// <summary>
    ///     Handles the inheritance of paragraph style settings. All settings that
    ///     have not been modified will be inherited from the base RtfParagraphStyle.
    ///     If this RtfParagraphStyle is not based on another one, then nothing happens.
    /// </summary>
    public void HandleInheritance()
    {
        if (_basedOnName != null && Document.GetDocumentHeader().GetRtfParagraphStyle(_basedOnName) != null)
        {
            _baseStyle = Document.GetDocumentHeader().GetRtfParagraphStyle(_basedOnName);
            _baseStyle.HandleInheritance();
            if (!((_modified & ModifiedAlignment) == ModifiedAlignment))
            {
                _alignment = _baseStyle.GetAlignment();
            }

            if (!((_modified & ModifiedIndentLeft) == ModifiedIndentLeft))
            {
                _indentLeft = _baseStyle.GetIndentLeft();
            }

            if (!((_modified & ModifiedIndentRight) == ModifiedIndentRight))
            {
                _indentRight = _baseStyle.GetIndentRight();
            }

            if (!((_modified & ModifiedSpacingBefore) == ModifiedSpacingBefore))
            {
                _spacingBefore = _baseStyle.GetSpacingBefore();
            }

            if (!((_modified & ModifiedSpacingAfter) == ModifiedSpacingAfter))
            {
                _spacingAfter = _baseStyle.GetSpacingAfter();
            }

            if (!((_modified & ModifiedFontName) == ModifiedFontName))
            {
                SetFontName(_baseStyle.GetFontName());
            }

            if (!((_modified & ModifiedFontSize) == ModifiedFontSize))
            {
                Size = _baseStyle.GetFontSize();
            }

            if (!((_modified & ModifiedFontStyle) == ModifiedFontStyle))
            {
                SetStyle(_baseStyle.GetFontStyle());
            }

            if (!((_modified & ModifiedFontColor) == ModifiedFontColor))
            {
                SetColor(_baseStyle.Color);
            }

            if (!((_modified & ModifiedLineLeading) == ModifiedLineLeading))
            {
                SetLineLeading(_baseStyle.GetLineLeading());
            }

            if (!((_modified & ModifiedKeepTogether) == ModifiedKeepTogether))
            {
                SetKeepTogether(_baseStyle.GetKeepTogether());
            }

            if (!((_modified & ModifiedKeepTogetherWithNext) == ModifiedKeepTogetherWithNext))
            {
                SetKeepTogetherWithNext(_baseStyle.GetKeepTogetherWithNext());
            }
        }
    }

    /// <summary>
    ///     Sets the alignment of this RtfParagraphStyle.
    /// </summary>
    /// <param name="alignment">The alignment to use.</param>
    public void SetAlignment(int alignment)
    {
        _modified = _modified | ModifiedAlignment;
        _alignment = alignment;
    }

    /// <summary>
    ///     Sets the colour of this RtfParagraphStyle.
    /// </summary>
    /// <param name="color">The Color to use.</param>
    public void SetColor(BaseColor color)
    {
        _modified = _modified | ModifiedFontColor;
        Color = color;
    }

    /// <summary>
    ///     Sets the first line indententation of this RtfParagraphStyle. It
    ///     is relative to the left indentation.
    /// </summary>
    /// <param name="firstLineIndent">The first line indentation to use.</param>
    public void SetFirstLineIndent(int firstLineIndent)
    {
        _firstLineIndent = firstLineIndent;
    }

    /// <summary>
    ///     Sets the font name of this RtfParagraphStyle.
    /// </summary>
    /// <param name="fontName">The font name to use</param>
    public override void SetFontName(string fontName)
    {
        _modified = _modified | ModifiedFontName;
        base.SetFontName(fontName);
    }

    /// <summary>
    ///     Sets the left indentation of this RtfParagraphStyle.
    /// </summary>
    /// <param name="indentLeft">The left indentation to use.</param>
    public void SetIndentLeft(int indentLeft)
    {
        _modified = _modified | ModifiedIndentLeft;
        _indentLeft = indentLeft;
    }

    /// <summary>
    ///     Sets the right indentation of this RtfParagraphStyle.
    /// </summary>
    /// <param name="indentRight">The right indentation to use.</param>
    public void SetIndentRight(int indentRight)
    {
        _modified = _modified | ModifiedIndentRight;
        _indentRight = indentRight;
    }

    /// <summary>
    ///     Sets whether the lines in the paragraph should be kept together in
    ///     this RtfParagraphStyle.
    /// </summary>
    /// <param name="keepTogether">Whether the lines in the paragraph should be kept together.</param>
    public void SetKeepTogether(bool keepTogether)
    {
        _keepTogether = keepTogether;
        _modified = _modified | ModifiedKeepTogether;
    }

    /// <summary>
    ///     Sets whether the paragraph should be kept together with the next in
    ///     this RtfParagraphStyle.
    /// </summary>
    /// <param name="keepTogetherWithNext">Whether the paragraph should be kept together with the next.</param>
    public void SetKeepTogetherWithNext(bool keepTogetherWithNext)
    {
        _keepTogetherWithNext = keepTogetherWithNext;
        _modified = _modified | ModifiedKeepTogetherWithNext;
    }

    /// <summary>
    ///     Sets the line leading of this RtfParagraphStyle.
    /// </summary>
    /// <param name="lineLeading">The line leading to use.</param>
    public void SetLineLeading(int lineLeading)
    {
        _lineLeading = lineLeading;
        _modified = _modified | ModifiedLineLeading;
    }

    /// <summary>
    ///     Sets the space after the paragraph of this RtfParagraphStyle.
    /// </summary>
    /// <param name="spacingAfter">The space after to use.</param>
    public void SetSpacingAfter(int spacingAfter)
    {
        _modified = _modified | ModifiedSpacingAfter;
        _spacingAfter = spacingAfter;
    }

    /// <summary>
    ///     Sets the space before the paragraph of this RtfParagraphStyle.
    /// </summary>
    /// <param name="spacingBefore">The space before to use.</param>
    public void SetSpacingBefore(int spacingBefore)
    {
        _modified = _modified | ModifiedSpacingBefore;
        _spacingBefore = spacingBefore;
    }

    /// <summary>
    ///     Sets the font style of this RtfParagraphStyle.
    /// </summary>
    /// <param name="style">The font style to use.</param>
    public override void SetStyle(int style)
    {
        _modified = _modified | ModifiedFontStyle;
        base.SetStyle(style);
    }

    /// <summary>
    ///     Writes the start information of this RtfParagraphStyle.
    ///     @throws IOException On i/o errors.
    /// </summary>
    /// <param name="result">The  OutputStream  to write to.</param>
    public override void WriteBegin(Stream result)
    {
        if (result == null)
        {
            throw new ArgumentNullException(nameof(result));
        }

        byte[] t;
        result.Write(t = DocWriter.GetIsoBytes("\\s"), 0, t.Length);
        result.Write(t = IntToByteArray(_styleNumber), 0, t.Length);
        writeParagraphSettings(result);
    }

    /// <summary>
    ///     unused
    /// </summary>
    public override void WriteContent(Stream outp)
    {
    }

    /// <summary>
    ///     Writes the definition of this RtfParagraphStyle for the stylesheet list.
    /// </summary>
    public override void WriteDefinition(Stream outp)
    {
        if (outp == null)
        {
            throw new ArgumentNullException(nameof(outp));
        }

        byte[] t;
        outp.Write(t = DocWriter.GetIsoBytes("{"), 0, t.Length);
        outp.Write(t = DocWriter.GetIsoBytes("\\style"), 0, t.Length);
        outp.Write(t = DocWriter.GetIsoBytes("\\s"), 0, t.Length);
        outp.Write(t = IntToByteArray(_styleNumber), 0, t.Length);
        outp.Write(t = RtfElement.Delimiter, 0, t.Length);
        writeParagraphSettings(outp);
        base.WriteBegin(outp);
        outp.Write(t = RtfElement.Delimiter, 0, t.Length);
        outp.Write(t = DocWriter.GetIsoBytes(_styleName), 0, t.Length);
        outp.Write(t = DocWriter.GetIsoBytes(";"), 0, t.Length);
        outp.Write(t = DocWriter.GetIsoBytes("}"), 0, t.Length);
        Document.OutputDebugLinebreak(outp);
    }

    /// <summary>
    ///     Unused
    /// </summary>
    public override void WriteEnd(Stream result)
    {
    }

    /// <summary>
    ///     Sets the number of this RtfParagraphStyle in the stylesheet list.
    /// </summary>
    /// <param name="styleNumber">The number to use.</param>
    protected internal void SetStyleNumber(int styleNumber)
    {
        _styleNumber = styleNumber;
    }

    /// <summary>
    ///     Gets the number of this RtfParagraphStyle in the stylesheet list.
    /// </summary>
    /// <returns>The number of this RtfParagraphStyle in the stylesheet list.</returns>
    private int getStyleNumber() => _styleNumber;

    /// <summary>
    ///     Writes the settings of this RtfParagraphStyle.
    /// </summary>
    private void writeParagraphSettings(Stream result)
    {
        byte[] t;
        if (_keepTogether)
        {
            result.Write(t = KeepTogether, 0, t.Length);
        }

        if (_keepTogetherWithNext)
        {
            result.Write(t = KeepTogetherWithNext, 0, t.Length);
        }

        switch (_alignment)
        {
            case Element.ALIGN_LEFT:
                result.Write(t = AlignLeft, 0, t.Length);
                break;
            case Element.ALIGN_RIGHT:
                result.Write(t = AlignRight, 0, t.Length);
                break;
            case Element.ALIGN_CENTER:
                result.Write(t = AlignCenter, 0, t.Length);
                break;
            case Element.ALIGN_JUSTIFIED:
            case Element.ALIGN_JUSTIFIED_ALL:
                result.Write(t = AlignJustify, 0, t.Length);
                break;
        }

        result.Write(t = FirstLineIndent, 0, t.Length);
        result.Write(t = IntToByteArray(_firstLineIndent), 0, t.Length);
        result.Write(t = IndentLeft, 0, t.Length);
        result.Write(t = IntToByteArray(_indentLeft), 0, t.Length);
        result.Write(t = IndentRight, 0, t.Length);
        result.Write(t = IntToByteArray(_indentRight), 0, t.Length);
        if (_spacingBefore > 0)
        {
            result.Write(t = SpacingBefore, 0, t.Length);
            result.Write(t = IntToByteArray(_spacingBefore), 0, t.Length);
        }

        if (_spacingAfter > 0)
        {
            result.Write(t = SpacingAfter, 0, t.Length);
            result.Write(t = IntToByteArray(_spacingAfter), 0, t.Length);
        }

        if (_lineLeading > 0)
        {
            result.Write(t = RtfPhrase.LineSpacing, 0, t.Length);
            result.Write(t = IntToByteArray(_lineLeading), 0, t.Length);
        }
    }
}